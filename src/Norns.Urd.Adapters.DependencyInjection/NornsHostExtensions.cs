using Microsoft.Extensions.DependencyInjection;
using Norns.Destiny.AOP;
using Norns.Urd.Loom;
using System;

namespace Microsoft.Extensions.Hosting
{
    public static class NornsHostExtensions
    {
        public static IHostBuilder UseUrdAop(this IHostBuilder builder, IInterceptorGenerator[] interceptors, LoomOptions options = null)
        {
            return builder.UseServiceProviderFactory(new UrdServiceProviderFactory()
            {
                Options = options,
                Interceptors = interceptors
            });
        }
    }

    public class UrdServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public LoomOptions Options { get; set; }

        public IInterceptorGenerator[] Interceptors { get; set; }

        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return containerBuilder.BuildUrdAopServiceProvider(Interceptors, Options);
        }
    }
}