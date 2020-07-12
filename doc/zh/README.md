# Norns

## 目标

把静态编织（static weaving）和动态编织（dynamic weaving）都做一遍.

## 设计

### 基于代理模式的AOP

0. 生成代理类型
1. 在di中替换为代理类型
2. 在代理类中实现AOP功能

### 静态编织Static weaving 基于[roslyn](https://github.com/dotnet/roslyn)生成代码

会尝试以下两种方式实现AOP :

#### AOT   (Norns.Skuld)

> 仍为实验特性

* 基于 [source-generators](https://github.com/dotnet/roslyn/blob/master/docs/features/source-generators.md) 生成代码

`
(ps: 目前由于 source-generators 不允许加载引用dll, 所以目前受限而无法作为公用包给大家使用.)
`

#### JIT     (Norns.Verthandi)

* 基于 Reflection 获取类型数据并生成代码 
* 基于 roslyn sdk 编译代码

```
不要在 production 使用该功能.

Roslyn 很强大, 但是依旧会消耗很多cpu和内存。
```

### Dynamic weaving   Emit  (Norns.Urd)

* 基于 Emit 生成代理类

> (ps: this will begin after static weaving done)


## How to use

### JIT     (Norns.Verthandi)

0. reference `Norns.Adapters.DependencyInjection`

1. write InterceptorGenerator base on `AbstractInterceptorGenerator`

```csharp
using Norns.Destiny.Structure;
using Norns.Destiny.AOP;
using Norns.Destiny.AOP.Notations;
using Norns.Destiny.Notations;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Benchmark
{
    public class ConsoleCallMethodGenerator : AbstractInterceptorGenerator
    {
        public override IEnumerable<INotation> BeforeMethod(ProxyGeneratorContext context, IMethodSymbolInfo method)
        {
            typeof(System.Console).Name.ToString(); // just make sure load System.Console dll before jit generate code
            if (!method.Parameters.IsEmpty)
            {
                yield return $"System.Console.WriteLine($\"Call Method {method.Name} at {{System.DateTime.Now.ToString(\"yyyy-MM-dd HH:mm:ss.fff\")}} {method.Parameters.First().Type.FullName} {method.Parameters.First().Name} = {{{method.Parameters.First().Name}}}".ToNotation();
                foreach (var item in method.Parameters.Skip(1))
                {
                    yield return $", {item.FullName} {item.Name} = {{{item.Name}}}".ToNotation();
                }
                yield return "\");".ToNotation();
            }
        }

        public override IEnumerable<INotation> AfterMethod(ProxyGeneratorContext context, IMethodSymbolInfo method)
        {
            if (method.HasReturnValue)
            {
                yield return $"System.Console.WriteLine($\"return {{{context.GetReturnValueParameterName()}}} at {{System.DateTime.Now.ToString(\"yyyy-MM-dd HH:mm:ss.fff\")}}\");".ToNotation();
            }
        }
    }
}
```

2. write interface 

```csharp
using System;

namespace Norns.Benchmark
{
    public interface IC
    {
        int AddOne(int v);

        public int AddOne2(int v)
        {
            return v + 1;
        }
    }
}
```

3. write class 

```csharp
public class C : IC
{
    public int AddOne(int v)
    {
        return v;
    }
}
```

4. set to DI

if use asp.net core, just use `UseVerthandiAop` like :

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .UseVerthandiAop(new IInterceptorGenerator[] { new ConsoleCallMethodGenerator() })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```

if not, you can try this :

```csharp
internal class Program
{
    private static void Main(string[] args)
    {
        var p = new ServiceCollection()
            .AddTransient<IC, C>()
            .BuildVerthandiAopServiceProvider(new IInterceptorGenerator[] { new ConsoleCallMethodGenerator() })
            .GetRequiredService<IC>();

        var result = p.AddOne(99);
        Console.WriteLine($"p.AddOne(99) 's result is {result}.");
        Console.WriteLine();
        result = p.AddOne2(1);
        Console.WriteLine($"p.AddOne2(1) 's result is {result}.");

        Console.ReadKey();
    }
}
```

result :

```shell
Call Method AddOne at 2020-07-05 15:42:21.999 int v = 99
return 99 at 2020-07-05 15:42:22.002
p.AddOne(99) 's result is 99.

Call Method AddOne2 at 2020-07-05 15:42:22.003 int v = 1
return 2 at 2020-07-05 15:42:22.003
p.AddOne2(1) 's result is 2.
```

### AOT   (Norns.Skuld)

source-generators 还有很多问题和变动，所以目前不提供demo 文档.

### Emit  (Norns.Urd)

暂未开始

## Plan 

- [ ] [Norns-JIT-Imporve](https://github.com/fs7744/Norns/projects/1)

## 取名缘由

Norns == 诺伦三女神 （北欧神话中的命运女神）

大女儿乌尔德（Urd）司掌“过去”，二女儿薇儿丹蒂（Verthandi）司掌“现在”，小女儿诗蔻蒂（Skuld）司掌“未来”。

所以借用该名希望该项目可以帮助大家 `自我掌控` 项目的“命运”。

