``` ini

BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.403
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|                                     Method |     Mean |     Error |    StdDev |    StdErr |      Min |       Q1 |   Median |       Q3 |      Max |         Op/s |
|------------------------------------------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|-------------:|
|          SyncMethod_HasParam_NoInterceptor | 11.16 ns | 0.2409 ns | 0.2135 ns | 0.0571 ns | 10.91 ns | 10.97 ns | 11.11 ns | 11.36 ns | 11.59 ns | 89,577,523.9 |
| SyncMethod_HasParam_HandWritingInterceptor | 13.56 ns | 0.1044 ns | 0.0976 ns | 0.0252 ns | 13.41 ns | 13.48 ns | 13.55 ns | 13.63 ns | 13.75 ns | 73,765,279.6 |
|    SyncMethod_HasParam_ProxyAndInterceptor | 62.33 ns | 0.9208 ns | 0.8614 ns | 0.2224 ns | 61.12 ns | 61.77 ns | 62.19 ns | 62.90 ns | 64.00 ns | 16,044,783.2 |
|           SyncMethod_NoParam_NoInterceptor | 18.90 ns | 0.1429 ns | 0.1267 ns | 0.0339 ns | 18.70 ns | 18.85 ns | 18.91 ns | 18.97 ns | 19.15 ns | 52,904,906.2 |
|  SyncMethod_NoParam_HandWritingInterceptor | 12.99 ns | 0.1591 ns | 0.1488 ns | 0.0384 ns | 12.81 ns | 12.88 ns | 12.93 ns | 13.14 ns | 13.25 ns | 76,991,278.7 |
|     SyncMethod_NoParam_ProxyAndInterceptor | 26.88 ns | 0.2234 ns | 0.2090 ns | 0.0540 ns | 26.59 ns | 26.71 ns | 26.85 ns | 27.04 ns | 27.25 ns | 37,201,558.2 |

SyncInterceptor 的结果为： 

|                                     Method |     Mean |     Error |    StdDev |    StdErr |      Min |       Q1 |   Median |       Q3 |      Max |         Op/s |
|------------------------------------------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|-------------:|
|    SyncMethod_HasParam_ProxyAndInterceptor | 45.53 ns | 0.9184 ns | 0.9827 ns | 0.2316 ns | 44.51 ns | 44.60 ns | 45.37 ns | 46.31 ns | 47.85 ns | 21,964,106.6 |

符合期望，dynamic 理应有更多处理，所以的确不适合这里