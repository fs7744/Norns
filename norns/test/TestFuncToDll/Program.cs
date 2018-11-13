using Norns.AOP.Configuration;
using Norns.AOP.Interceptors;
using Norns.Core.AOP.Configuration;
using Norns.DependencyInjection;

namespace TestFuncToDll
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new ServiceDefintions()
                .AddSingleton<IInterceptorConfiguration>(i =>
                {
                    var c = new InterceptorConfiguration();
                    c.Interceptors.Add(new TestInterceptor());
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
               .AddSingleton<ISyncFunc, SyncFunc>()
               .BuildServiceProvider()
               .GetRequiredService<ISyncFunc>()
               .SyncCallNoParameters();
        }
    }
}