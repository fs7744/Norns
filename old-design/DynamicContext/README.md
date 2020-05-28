# DynamicContext

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

# ExpandoContext

``` ini

BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.403
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|                                     Method |        Mean |      Error |     StdDev |    StdErr |         Min |          Q1 |      Median |          Q3 |         Max |         Op/s |
|------------------------------------------- |------------:|-----------:|-----------:|----------:|------------:|------------:|------------:|------------:|------------:|-------------:|
|          SyncMethod_HasParam_NoInterceptor |    11.38 ns |  0.1417 ns |  0.1325 ns | 0.0342 ns |    11.21 ns |    11.27 ns |    11.35 ns |    11.46 ns |    11.69 ns | 87,840,205.4 |
| SyncMethod_HasParam_HandWritingInterceptor |    13.36 ns |  0.2926 ns |  0.2594 ns | 0.0693 ns |    13.06 ns |    13.16 ns |    13.28 ns |    13.53 ns |    14.01 ns | 74,849,649.6 |
|    SyncMethod_HasParam_ProxyAndInterceptor | 1,249.44 ns | 15.3168 ns | 13.5779 ns | 3.6288 ns | 1,231.37 ns | 1,237.32 ns | 1,250.90 ns | 1,255.73 ns | 1,276.93 ns |    800,359.0 |
|           SyncMethod_NoParam_NoInterceptor |    19.83 ns |  0.2283 ns |  0.1906 ns | 0.0529 ns |    19.48 ns |    19.72 ns |    19.78 ns |    20.01 ns |    20.15 ns | 50,421,023.5 |
|  SyncMethod_NoParam_HandWritingInterceptor |    13.16 ns |  0.1915 ns |  0.1697 ns | 0.0454 ns |    12.92 ns |    13.03 ns |    13.14 ns |    13.23 ns |    13.54 ns | 75,995,335.6 |
|     SyncMethod_NoParam_ProxyAndInterceptor |    25.06 ns |  0.2796 ns |  0.2478 ns | 0.0662 ns |    24.63 ns |    24.89 ns |    25.09 ns |    25.19 ns |    25.47 ns | 39,906,412.0 |


# object convert test 

结果为： 

|                                     Method |     Mean |     Error |    StdDev |    StdErr |      Min |       Q1 |   Median |       Q3 |      Max |         Op/s |
|------------------------------------------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|-------------:|
|    SyncMethod_HasParam_ProxyAndInterceptor | 45.53 ns | 0.9184 ns | 0.9827 ns | 0.2316 ns | 44.51 ns | 44.60 ns | 45.37 ns | 46.31 ns | 47.85 ns | 21,964,106.6 |

符合期望，dynamic 理应有更多处理，所以的确不适合这里