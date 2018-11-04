using Microsoft.Extensions.DependencyInjection;
using System;

namespace Norns.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider BuildServiceProvider(this IServiceCollection services, ServiceProviderOptions options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return new ServiceProvider(services, options ?? ServiceProviderOptions.Default);
        }
    }
}