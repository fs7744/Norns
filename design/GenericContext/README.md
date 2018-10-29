# GenericContext
``` ini

BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.403
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|                                        Method |     Mean |     Error |    StdDev |    StdErr |      Min |       Q1 |   Median |       Q3 |      Max |         Op/s |
|---------------------------------------------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|-------------:|
|             SyncMethod_HasParam_NoInterceptor | 11.28 ns | 0.1461 ns | 0.1366 ns | 0.0353 ns | 11.06 ns | 11.17 ns | 11.29 ns | 11.41 ns | 11.49 ns | 88,637,771.4 |
|    SyncMethod_HasParam_HandWritingInterceptor | 13.83 ns | 0.2048 ns | 0.1915 ns | 0.0495 ns | 13.46 ns | 13.71 ns | 13.90 ns | 13.95 ns | 14.12 ns | 72,306,351.6 |
|       SyncMethod_HasParam_ProxyAndInterceptor | 27.46 ns | 0.3590 ns | 0.3359 ns | 0.0867 ns | 26.99 ns | 27.21 ns | 27.38 ns | 27.69 ns | 28.16 ns | 36,420,828.9 |
| SyncMethod_HasParam_Tuple_ProxyAndInterceptor | 30.59 ns | 0.4684 ns | 0.4382 ns | 0.1131 ns | 29.91 ns | 30.32 ns | 30.52 ns | 30.97 ns | 31.36 ns | 32,691,528.2 |
|              SyncMethod_NoParam_NoInterceptor | 19.64 ns | 0.1461 ns | 0.1366 ns | 0.0353 ns | 19.43 ns | 19.49 ns | 19.66 ns | 19.72 ns | 19.94 ns | 50,908,184.2 |
|     SyncMethod_NoParam_HandWritingInterceptor | 13.24 ns | 0.0897 ns | 0.0839 ns | 0.0217 ns | 13.11 ns | 13.17 ns | 13.23 ns | 13.30 ns | 13.44 ns | 75,539,187.4 |
|        SyncMethod_NoParam_ProxyAndInterceptor | 24.66 ns | 0.2392 ns | 0.2238 ns | 0.0578 ns | 24.28 ns | 24.51 ns | 24.64 ns | 24.82 ns | 25.09 ns | 40,553,123.9 |

# object convert test 

结果为： 

|                                     Method |     Mean |     Error |    StdDev |    StdErr |      Min |       Q1 |   Median |       Q3 |      Max |         Op/s |
|------------------------------------------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|-------------:|
|    SyncMethod_HasParam_ProxyAndInterceptor | 45.53 ns | 0.9184 ns | 0.9827 ns | 0.2316 ns | 44.51 ns | 44.60 ns | 45.37 ns | 46.31 ns | 47.85 ns | 21,964,106.6 |



对比而言，性能是要十多纳秒，

但目前还没想到如何兼顾写拦截器时的友好性以及节约这十多纳秒