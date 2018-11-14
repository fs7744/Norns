# Norns

## 项目目的

该项目核心目的为探索 dotnet core AOP静态编织 之路。

次要目的提供一套可用的框架 （如果无人会使用，至少也希望可以成为AOP静态编织的可参考资料）。

## 取名缘由

Norns == 诺伦三女神 （北欧神话中的命运女神）

大女儿乌尔德（Urd）司掌“过去”，二女儿薇儿丹蒂（Verthandi）司掌“现在”，小女儿诗蔻蒂（Skuld）司掌“未来”。

所以借用该名希望该项目可以帮助大家 `自我掌控` 项目的“命运”。

## 设计

AOP静态编织 分为两条道路：

* 使用预处理器添加源代码 c++等中存在类似宏的处理 
* 使用后处理器在编译后的二进制代码上添加指令， dotnet 里面可以叫做 IL 重写。

C# 里面有三种方式可以做静态编织：

1. 可以使用Roslyn 在编译时修改AST添加拦截的代码  (不过暂时我没有在msbuild task的文档中找到怎么添加Roslyn 编译时修改，倒是可以自己利用Roslyn自己做编译，感觉这工作量有点大，有同学如果知道怎么添加Roslyn 编译时修改请一定要告知我，thanks very much)
2. 使用mono.cecli或者dnlib 在编译后进行IL重写
3. 实现CLR Profiling API 在JIT编译时修改method body。实现真正无任何限制的运行时静态AOP  （不过貌似得用C++才能做CLR Profiling API）

### 对比思考

<table class="tg">
  <tr>
    <th class="tg-0pky"></th>
    <th class="tg-0pky"><b>代码生成</b></th>
    <th class="tg-0pky"><b>IL 重写</b></th>
  </tr>
  <tr>
    <td class="tg-0pky"><b>代码可见</b></td>
    <td class="tg-0pky">√</td>
    <td class="tg-0pky">×</td>
  </tr>
  <tr>
    <td class="tg-0pky"><b>语法检查或优化</b></td>
    <td class="tg-0pky">√ （ide或插件可以运用）</td>
    <td class="tg-0pky">× （依赖框架实现者IL 功力）</td>
  </tr>
  <tr>
    <td class="tg-0pky"><b>实现方式</b></td>
    <td class="tg-0pky">√× （代理模式，避免影响用户源码，主要是目前编译器没c++那种创建宏之类的能力，只能替换）</td>
    <td class="tg-0pky">√ （直接修改IL源码，不影响用户源码）</td>
  </tr>
  <tr>
    <td class="tg-0pky"><b>static 支持</b></td>
    <td class="tg-0pky">×  （避免影响用户源码）</td>
    <td class="tg-0pky">√ </td>
  </tr>
  <tr>
    <td class="tg-0pky"><b>任意方法，属性 支持</b></td>
    <td class="tg-0pky">×  （只支持重载的方法，属性，类等）</td>
    <td class="tg-0pky">√ </td>
  </tr>
  <tr>
    <td class="tg-0pky"><b>aop 拦截器模式</b></td>
    <td class="tg-0pky">√ （可用 中间件思路交由用户自我控制，框架实现要简单些，使用者学习成本低些）</td>
    <td class="tg-0pky">√× （由于IL编写的复杂度以及对应优化等，通常拆分较细，框架实现内容要多些，使用者学习成本高些）</td>
  </tr>
  <tr>
    <td class="tg-0pky"><b>async/await 支持</b></td>
    <td class="tg-0pky">√ （较为简单）</td>
    <td class="tg-0pky">√× （修改IL的状态机代码+添加aop逻辑还是挺复杂的）</td>
  </tr>
  <tr>
    <td class="tg-0pky"><b>ioc 支持</b></td>
    <td class="tg-0pky">√x （依赖ioc的实现类替换，不同ioc要进行适配，需要一点代码入侵）</td>
    <td class="tg-0pky">√ （因直接修改il源码，对ioc毫不相干）</td>
  </tr>
  <tr>
    <td class="tg-0pky"><b>无源码，如只有dll的动态拦截支持</b></td>
    <td class="tg-0pky">√x （使用ioc的情况，可以依赖ioc用代理类替换，需要一点代码入侵；不使用ioc，用户可以直接用代理类，无法使用的情况就毫无办法了）</td>
    <td class="tg-0pky">√ （可以直接修改dll，不过一般不推荐这么做，因为部署方式差异，可能导致修改不了，或者修改了之后导致dll无法共用，其他程序出错，所以一般做运行时特殊处理）</td>
  </tr>
    <tr>
    <td class="tg-0pky"><b>性能</b></td>
    <td class="tg-0pky">√ （理论与IL重写应该相差不大，即使IL可以进一步优化，也应该在纳秒级别的差异）</td>
    <td class="tg-0pky">√ </td>
  </tr>
</table>

## 设计

``` csharp
//原始代码
public class Test
{
    [AddOne]
    public int Sum(int x, int y)
    {
      return x + y;
    } 
}

//Aop拦截器
public class AddOneAttribute : InterceptorBaseAttribute
{
    public override void Intercept(InterceptContext context, InterceptDelegate next)
    {
        Console.WriteLine("Begin");
        next(context);
        Console.WriteLine("End");
    }
}

//静态编织生成的类大致样子
[NoIntercept]
public class Test 
{
    private static MethodInfo Sum_guidMethod = typeof(TestSumService).GetMethod("Sum_guid", new Type[] { typeof(int), typeof(int) }); //避免多次获取，所以用static
    private readonly InterceptDelegate Sum_guidInterceptor;  //缓存类似中间件的delegate，避免方法内部使用实例属性字段，所以不用static

    public Test(IInterceptDelegateBuilder builder)  //修改构造器，以便创建中间件的delegate
    {
      Sum_guidInterceptor = builder.BuildInterceptDelegate(Sum_guidMethod, c =>
      {
          c.Result = Sum_guid((int)c.Parameters[0], (int)c.Parameters[1]);
      });
    }

    [AddOne]    
    public int Sum_guid(int x, int y) //将原始方法创建成新方法，以便代理方法调用
    {
      return x + y;
    } 

    public int Sum(int x, int y)  // 替换原始方法为代理方法
    {
      var context = new InterceptContext()
      {
          ServiceMethod = Sum_guidMethod,
          Parameters = new object[] { x, y }
      };
      Sum_guidInterceptor(context);
      return (int)context.Result;
    } 
}

// 总体虽然用静态编织，但实际依然时代理模式
// 这是我目前想到的唯一兼容动态代理AOP，以便支持动态全局拦截的方式
// 如果大家有更好方式，请一定要告诉我
```

## Roadmap

- 代码生成 实现
    - 探索与设计 阶段
        - nuget 包编译命令注入简单研究 （✔） [design/TestMSBuild](design/TestMSBuild)
        - 同步拦截器+代理类设计以及性能简单对比 （✔）[design/SyncInterceptor](design/SyncInterceptor)
        - 异步拦截器+代理类设计以及性能简单对比 （✔）[design/AsyncInterceptor](design/AsyncInterceptor)
        - 拦截器上下文如何尽量避免类型转换，更加泛型设计探索 （✔）[design/DynamicContext](design/DynamicContext) vs [design/GenericContext](design/GenericContext) *(object 看来依然是能想到的拦截器通用设计方式中 性能与友好性最好的方式)*
        - IOC适配探索 （✔）[design/TestIOC](design/TestIOC)
        - asp.net core Controller 代理+IOC适配探索 （✔）[design/TestIOC](design/TestIOC) *(asp.net core框架必须特殊适配，但任何框架都可能有类似的特殊处理导致替换机制无法适用，全部适配代价太大)*
        - 全局拦截器 （✔）*(只有通过ioc才能统一掌控所有实例的创建，才能做到动态添加代理类，这样才能动态创建全局拦截器，微软或者其他的ioc实现并不一定包含aop所需功能，综合来看必须做个ioc实现)*
        - roslyn 解析代码 （✔）*(无法准确完整识别继承关系)*
    - 实现 阶段
        - 拦截器（同步+异步） 编写 （✔）
        - 拦截器筛选机制 编写 （✔）
        - 筛选机制扩展方法 编写（✔）
        - 拦截器动态创建优化+全局拦截器配置扩展 编写（✔）
        - 特性拦截器动态创建 编写（✔）
        - 静态拦截代理类代码 生成 编写 （✔）*(纯解析代码导致很多处理不准确，整体来看从源代码去做代码生成只适合固定且最好简单的格式代码)*
        - *(纯解析代码不准确性很难解决，现修改计划，先DI然后实现IL重写，最后实现动态代理以支持全局拦截)* （✔）
- DependencyInjection 编写
    - DI Singleton 编写 （✔）
    - DI Scoped 编写 （✔）
    - DI Transient 编写 （✔）
    - DI 属性注入 （✔）
    - DI named dependency （✔）
    - DI 适配 Microsoft.Extensions.DependencyInjection （✔）
- IL 重写 实现 
    - 初步IL重写测试  （✔）
    - nuget 项目编译适配器 编写
- 动态代理类 生成 编写
- 示例 编写
- 文档 编写

## 初步IL重写测试结果
测试代码参见  https://github.com/fs7744/Norns/tree/master/norns/test/TestFuncToDll 和 https://github.com/fs7744/Norns/blob/master/norns/test/Norns.Test/StaticWeave/AssemblyScannerTest.cs

``` ini

BenchmarkDotNet=v0.11.2, OS=Windows 10.0.17134.407 (1803/April2018Update/Redstone4)
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.403
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|                    Method |      Mean |     Error |    StdDev |
|-------------------------- |----------:|----------:|----------:|
|            RealNewAndCall |  60.97 ns | 0.1618 ns | 0.1434 ns |
|           ProxyNewAndCall | 308.39 ns | 1.7986 ns | 1.5944 ns |
| AspectCoreProxyNewAndCall | 582.90 ns | 2.2300 ns | 1.9769 ns |
|              RealJustCall |  15.95 ns | 0.0882 ns | 0.0737 ns |
|             ProxyJustCall | 115.55 ns | 0.2246 ns | 0.1991 ns |
|   AspectCoreProxyJustCall | 310.25 ns | 0.8924 ns | 0.7911 ns |

`和aspectcore 的时间差异估计应该是 aspectcore 将同步方法转化为异步方法的差异`


# 鸣谢列表

* [https://github.com/dotnetcore/AspectCore-Framework](https://github.com/dotnetcore/AspectCore-Framework) 从中学习(抄袭)了很多，感谢其优雅实现和代码，获益良多
