﻿using Microsoft.Extensions.DependencyInjection;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOP;
using Norns.Destiny.Attributes;
using Norns.Destiny.JIT.AOP;
using Norns.Destiny.JIT.Coder;
using Norns.Destiny.JIT.Structure;
using Norns.Destiny.UT.AOT.AOP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.UT.JIT.AOP
{
    public static class JitTest
    {
        private static JitOptions options = JitOptions.CreateDefault();

        public static Dictionary<string, TypeSymbolInfo> Generate(params Type[] types)
        {
            var generator = new OnlyDefaultImplementNotationGenerator(options, new IInterceptorGenerator[] { new EmptyInterceptorGenerator() });
            var assembly = generator.Generate(new JitTypesSymbolSource(types));
            return assembly.GetTypes().Select(i => i.GetSymbolInfo() as TypeSymbolInfo).Where(i => i.HasAttribute<DefaultImplementAttribute>())
                .ToDictionary(i => i.Name, i => i);
        }

        public static T GenerateProxy<T>()
        {
            return GenerateProxy<T>(typeof(T));
        }

        public static R GenerateProxy<R>(Type type)
        {
            var generator = new JitAopSourceGenerator(options, new IInterceptorGenerator[] { new AddSomeTingsInterceptorGenerator() });
            var assembly = generator.Generate(new JitTypesSymbolSource(type));
            var services = new ServiceCollection();
            services.AddDestinyInterface<R>();
            var provider = services.BuildAopServiceProvider(assembly);
            return provider.GetRequiredService<R>();
        }
    }
}