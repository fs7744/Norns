``` ini

BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.403
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|                                     Method |     Mean |     Error |    StdDev |    StdErr |      Min |       Q1 |   Median |       Q3 |      Max |         Op/s |
|------------------------------------------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|-------------:|
|          SyncMethod_HasParam_NoInterceptor | 10.86 ns | 0.1157 ns | 0.1082 ns | 0.0279 ns | 10.76 ns | 10.78 ns | 10.80 ns | 10.96 ns | 11.09 ns | 92,046,855.2 |
| SyncMethod_HasParam_HandWritingInterceptor | 13.40 ns | 0.1723 ns | 0.1612 ns | 0.0416 ns | 13.22 ns | 13.26 ns | 13.37 ns | 13.52 ns | 13.72 ns | 74,634,643.8 |
|    SyncMethod_HasParam_ProxyAndInterceptor | 45.53 ns | 0.9184 ns | 0.9827 ns | 0.2316 ns | 44.51 ns | 44.60 ns | 45.37 ns | 46.31 ns | 47.85 ns | 21,964,106.6 |
|           SyncMethod_NoParam_NoInterceptor | 18.80 ns | 0.2971 ns | 0.2779 ns | 0.0718 ns | 18.52 ns | 18.58 ns | 18.73 ns | 19.08 ns | 19.34 ns | 53,185,495.5 |
|  SyncMethod_NoParam_HandWritingInterceptor | 13.03 ns | 0.1973 ns | 0.1845 ns | 0.0476 ns | 12.82 ns | 12.84 ns | 13.12 ns | 13.15 ns | 13.31 ns | 76,755,935.3 |
|     SyncMethod_NoParam_ProxyAndInterceptor | 26.35 ns | 0.4827 ns | 0.4515 ns | 0.1166 ns | 25.96 ns | 26.05 ns | 26.14 ns | 26.66 ns | 27.34 ns | 37,947,596.7 |


结果来看， 值类型的拆装箱消耗还是有比较多消耗的