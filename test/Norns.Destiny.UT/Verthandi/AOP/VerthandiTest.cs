using Microsoft.Extensions.DependencyInjection;
using Norns.Destiny.AOP;
using Norns.Destiny.Attributes;
using Norns.Destiny.RuntimeSymbol;
using Norns.Destiny.Structure;
using Norns.Destiny.UT.Skuld.AOP;
using Norns.Verthandi.AOP;
using Norns.Verthandi.Loom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.UT.Verthandi.AOP
{
    public static class VerthandiTest
    {
        private static LoomOptions options = LoomOptions.CreateDefault();

        public static Dictionary<string, TypeSymbolInfo> Generate(params Type[] types)
        {
            var generator = new OnlyDefaultImplementNotationGenerator(options, new IInterceptorGenerator[] { new EmptyInterceptorGenerator() });
            var assembly = generator.Generate(new TypesSymbolSource(types));
            return assembly.GetTypes().Select(i => i.GetSymbolInfo() as TypeSymbolInfo).Where(i => i.HasAttribute<DefaultImplementAttribute>())
                .ToDictionary(i => i.Name, i => i);
        }

        public static T GenerateProxy<T>(ServiceLifetime lifetime = ServiceLifetime.Singleton, Action<IServiceCollection> action = null)
        {
            return GenerateProxy<T>(typeof(T), lifetime, action);
        }

        public static R GenerateProxy<R>(Type type, ServiceLifetime lifetime = ServiceLifetime.Singleton, Action<IServiceCollection> action = null)
        {
            var generator = new AopSourceGenerator(options, new IInterceptorGenerator[] { new AddSomeTingsInterceptorGenerator() });
            var assembly = generator.Generate(new TypesSymbolSource(type));
            var services = new ServiceCollection();
            if (action == null)
            {
                services.AddDestinyInterface<R>(lifetime);
            }
            else
            {
                action(services);
            }
            var provider = services.BuildAopServiceProvider(assembly);
            return provider.GetRequiredService<R>();
        }
    }
}