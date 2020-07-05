# Norns

## Goal

This a project to do static weaving and dynamic weaving.

## Desgin

### AOP base on proxy

0. Generate proxy class type
1. Replace type to proxy type for di
2. Do aop in proxy class

### Static weaving generate code base on [roslyn](https://github.com/dotnet/roslyn)

There is two way that we will try to support :

#### AOT

> experimental feature

* use [source-generators](https://github.com/dotnet/roslyn/blob/master/docs/features/source-generators.md) to generator proxy class code 

`
(ps: because source-generators not allow loading referenced assemblies now, so can't share package to other now. I will try to find way to fix this.)
`

#### JIT

* use Reflection to generator proxy class code 
* use roslyn sdk to convert code to type

```
Roslyn is so great, but if we just use it once before di, it seem wasting a lot of memory and cpu.

Actually we can do jit after generate dll to generate proxy dll, make the jit to aot after build.

But now there is source-generators.
```

### Dynamic weaving

* Emit to generate proxy type

> (ps: this will begin after static weaving done)


## How to use

### JIT

0. reference `Norns.Adapters.DependencyInjection`

1. write InterceptorGenerator base on `AbstractInterceptorGenerator`

```csharp
using Norns.Destiny.Abstraction.Structure;
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
                yield return $"System.Console.WriteLine($\"Call Method {method.Name} at {{System.DateTime.Now.ToString(\"yyyy-MM-dd HH:mm:ss.fff\")}} {method.Parameters[0].Type.FullName} {method.Parameters[0].Name} = {{{method.Parameters[0].Name}}}".ToNotation();
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
using Norns.Destiny.Attributes;
using System;

namespace Norns.Benchmark
{
    [Charon]
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

```csharp
internal class Program
{
    private static void Main(string[] args)
    {
        var p = new ServiceCollection()
            .AddTransient<IC, C>()
            .BuildJitAopServiceProvider(null, new IInterceptorGenerator[] { new ConsoleCallMethodGenerator() }, AppDomain.CurrentDomain.GetAssemblies())
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

### AOT

There is Noting until source-generators can do this.

Because source-generators not allow loading referenced assemblies now, so can't share package to other, and i don't want to write the demo.

### Emit

waiting to start

## 取名缘由

Norns == 诺伦三女神 （北欧神话中的命运女神）

大女儿乌尔德（Urd）司掌“过去”，二女儿薇儿丹蒂（Verthandi）司掌“现在”，小女儿诗蔻蒂（Skuld）司掌“未来”。

所以借用该名希望该项目可以帮助大家 `自我掌控` 项目的“命运”。

