using BenchmarkDotNet.Attributes;
using Norns.AOP.Configuration;
using Norns.AOP.Interceptors;
using Norns.Core.AOP.Configuration;
using Norns.DependencyInjection;
using System;

namespace TestFuncToDll
{
    [Norns.AOP.Attributes.NoIntercept]
    public class Test
    {
        private readonly INamedServiceProvider ioc;
        private readonly ISyncFunc2 real;
        private readonly ISyncFunc proxy;

        public Test()
        {
            ioc = new ServiceDefintions()
                .AddSingleton<IInterceptorConfiguration>(i =>
                {
                    var c = new InterceptorConfiguration();
                    //c.Interceptors.Add(new TestInterceptor());
                    return c;
                })
                .AddTransient<IInterceptorConfigurationHandler>(i =>
                {
                    return new InterceptorAttributeConfigurationHandler(null);
                })
                .AddTransient<IInterceptorCreatorFactory, InterceptorCreatorFactory>()
                .AddSingleton<IInterceptDelegateBuilder>(i =>
                {
                    return i.GetRequiredService<IInterceptorCreatorFactory>().Build();
                })
               .AddTransient<ISyncFunc, SyncFunc>()
               .AddTransient<ISyncFunc2, SyncFunc2>()
               .BuildServiceProvider();

            real = ioc.GetRequiredService<ISyncFunc2>();
            real.SyncCallNoParameters();
            proxy = ioc.GetRequiredService<ISyncFunc>();
            proxy.SyncCallNoParameters();
        }

        [Benchmark]
        public void RealNewAndCall()
        {
            ioc.GetRequiredService<ISyncFunc2>().SyncCallNoParameters();
        }

        [Benchmark]
        public void ProxyNewAndCall()
        {
            ioc.GetRequiredService<ISyncFunc>().SyncCallNoParameters();
        }

        [Benchmark]
        public void RealJustCall()
        {
            real.SyncCallNoParameters();
        }

        [Benchmark]
        public void ProxyJustCall()
        {
            proxy.SyncCallNoParameters();
        }
    }
}