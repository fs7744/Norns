using Norns.AOP.Configuration;
using Norns.Core.AOP.Configuration;
using Norns.AOP.Interceptors;
using Norns.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Norns.Test.Interceptors
{
    public class GlobalInterceptorTest
    {
        public class AddOne2 : InterceptorBase
        {
            public override async Task InterceptAsync(InterceptContext context, AsyncInterceptDelegate nextAsync)
            {
                await nextAsync(context);
                context.Result = (int)context.Result + 1;
            }
        }

        [Fact]
        public void CreateDelegateBuilder()
        {
            var builder = new ServiceDefintions()
                .AddSingleton<IInterceptorConfiguration>(i =>
                {
                    var c = new InterceptorConfiguration();
                    c.Interceptors.AddType<AddOne2>();
                    c.Interceptors.AddType<AddOne2>(whitelists: m => m != null);
                    c.Interceptors.AddType<AddOne2>(blacklists: m => m != null);
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
                .BuildServiceProvider()
                .GetRequiredService<IInterceptDelegateBuilder>();
            Assert.NotNull(builder);
        }
    }
}