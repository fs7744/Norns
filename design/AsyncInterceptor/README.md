``` ini

BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.403
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|                                                  Method |      Mean |     Error |    StdDev |    StdErr |       Min |        Q1 |    Median |        Q3 |       Max |         Op/s |
|-------------------------------------------------------- |----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|-------------:|
|                       SyncMethod_HasParam_NoInterceptor |  19.22 ns | 0.0777 ns | 0.0726 ns | 0.0188 ns |  19.07 ns |  19.17 ns |  19.21 ns |  19.28 ns |  19.33 ns | 52,037,096.7 |
|              SyncMethod_HasParam_HandWritingInterceptor |  12.64 ns | 0.1334 ns | 0.1248 ns | 0.0322 ns |  12.46 ns |  12.53 ns |  12.65 ns |  12.74 ns |  12.85 ns | 79,103,272.0 |
|                 SyncMethod_HasParam_ProxyAndInterceptor |  52.93 ns | 0.6332 ns | 0.5613 ns | 0.1500 ns |  52.09 ns |  52.39 ns |  52.93 ns |  53.32 ns |  53.98 ns | 18,891,425.9 |
|                      AsyncMethod_HasParam_NoInterceptor |  34.33 ns | 0.4217 ns | 0.3944 ns | 0.1018 ns |  33.80 ns |  33.97 ns |  34.25 ns |  34.63 ns |  35.22 ns | 29,131,069.4 |
|             AsyncMethod_HasParam_HandWritingInterceptor |  51.82 ns | 0.5136 ns | 0.4289 ns | 0.1190 ns |  51.39 ns |  51.45 ns |  51.69 ns |  52.07 ns |  52.81 ns | 19,296,092.0 |
|        AsyncMethod_HasParam_NoAwait_ProxyAndInterceptor |  73.80 ns | 1.4976 ns | 1.5380 ns | 0.3730 ns |  72.04 ns |  72.65 ns |  72.99 ns |  74.89 ns |  77.12 ns | 13,549,538.2 |
|                AsyncMethod_HasParam_ProxyAndInterceptor | 127.90 ns | 2.0315 ns | 1.9003 ns | 0.4906 ns | 125.92 ns | 126.29 ns | 127.35 ns | 128.79 ns | 131.86 ns |  7,818,575.2 |
| SyncMethod_HasParam_SyncBaseOnAsync_ProxyAndInterceptor |  87.87 ns | 1.1811 ns | 1.1048 ns | 0.2853 ns |  86.68 ns |  86.97 ns |  87.53 ns |  88.88 ns |  90.44 ns | 11,380,732.9 |



Sync 和 Async 表明task 以及 async/await 大致有几十纳秒的消耗，

这个消耗并不是特别大，对偷懒的同学，我们可以让偷懒的同学只实现异步拦截器

同步拦截器调异步拦截器就好，

关心性能的同学让其同时实现同步和异步拦截器