# GenericContext
``` ini

BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.403
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|                                                         Method |     Mean |     Error |    StdDev |    StdErr |      Min |       Q1 |   Median |       Q3 |      Max |         Op/s |
|--------------------------------------------------------------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|-------------:|
|                              SyncMethod_HasParam_NoInterceptor | 11.48 ns | 0.0942 ns | 0.0881 ns | 0.0227 ns | 11.36 ns | 11.40 ns | 11.46 ns | 11.54 ns | 11.68 ns | 87,137,217.5 |
|                     SyncMethod_HasParam_HandWritingInterceptor | 13.04 ns | 0.1272 ns | 0.1128 ns | 0.0301 ns | 12.91 ns | 12.95 ns | 13.05 ns | 13.12 ns | 13.30 ns | 76,685,228.4 |
|                        SyncMethod_HasParam_ProxyAndInterceptor | 27.03 ns | 0.1955 ns | 0.1829 ns | 0.0472 ns | 26.71 ns | 26.89 ns | 27.04 ns | 27.17 ns | 27.36 ns | 37,000,961.0 |
|                  SyncMethod_HasParam_Tuple_ProxyAndInterceptor | 30.33 ns | 0.5041 ns | 0.4716 ns | 0.1218 ns | 29.79 ns | 29.98 ns | 30.20 ns | 30.64 ns | 31.30 ns | 32,975,122.8 |
| SyncMethod_HasParam_TupleWithObjectConvert_ProxyAndInterceptor | 55.73 ns | 0.4132 ns | 0.3451 ns | 0.0957 ns | 54.99 ns | 55.58 ns | 55.80 ns | 55.92 ns | 56.41 ns | 17,944,094.7 |
|                               SyncMethod_NoParam_NoInterceptor | 11.64 ns | 0.0878 ns | 0.0778 ns | 0.0208 ns | 11.53 ns | 11.58 ns | 11.64 ns | 11.67 ns | 11.84 ns | 85,896,801.3 |
|                      SyncMethod_NoParam_HandWritingInterceptor | 12.87 ns | 0.0781 ns | 0.0692 ns | 0.0185 ns | 12.77 ns | 12.83 ns | 12.85 ns | 12.94 ns | 13.00 ns | 77,690,759.8 |
|                         SyncMethod_NoParam_ProxyAndInterceptor | 25.30 ns | 0.2738 ns | 0.2427 ns | 0.0649 ns | 25.00 ns | 25.08 ns | 25.27 ns | 25.50 ns | 25.68 ns | 39,522,706.2 |

# object convert test 

结果为： 

|                                     Method |     Mean |     Error |    StdDev |    StdErr |      Min |       Q1 |   Median |       Q3 |      Max |         Op/s |
|------------------------------------------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|-------------:|
|    SyncMethod_HasParam_ProxyAndInterceptor | 45.53 ns | 0.9184 ns | 0.9827 ns | 0.2316 ns | 44.51 ns | 44.60 ns | 45.37 ns | 46.31 ns | 47.85 ns | 21,964,106.6 |



对比而言，性能是要十多纳秒，

暂无方法兼顾写拦截器时的友好性以及节约这十多纳秒

大部分时候写拦截器时无法确定参数以及返回值类型，

即使proxy中确定了所有类型，

写通用拦截器时依然只能从object开始处理，

所以从tuple获取object -> object 转换为真实类型时，反而产生了更多了装箱操作，性能更低

object 看来依然是能想到的拦截器通用设计方式中 性能与友好性最好的方式