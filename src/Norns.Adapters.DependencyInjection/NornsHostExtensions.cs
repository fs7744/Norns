using Microsoft.Extensions.DependencyInjection;
using Norns.Destiny.AOP;
using Norns.Verthandi.Loom;
using System;

namespace Microsoft.Extensions.Hosting
{
    public static class NornsHostExtensions
    {
        public static IHostBuilder UseVerthandiAop(this IHostBuilder builder, IInterceptorGenerator[] interceptors, LoomOptions options = null)
        {
            return builder.UseServiceProviderFactory(new VerthandiServiceProviderFactory()
            {
                Options = options,
                Interceptors = interceptors
            });
        }
    }

    public class VerthandiServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public LoomOptions Options { get; set; }
        public IInterceptorGenerator[] Interceptors { get; set; }

        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return containerBuilder.BuildVerthandiAopServiceProvider(Interceptors, Options);
        }
    }
}