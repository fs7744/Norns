using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.AOP;
using Norns.Destiny.AOP.Notations;
using Norns.Destiny.JIT.Coder;
using System.Collections.Generic;

namespace Norns.Destiny.JIT.AOP
{
    public class JitAopSourceGenerator : JitAssemblyGenerator
    {
        protected readonly JitOptions options;
        protected readonly IEnumerable<IInterceptorGenerator> generators;

        public JitAopSourceGenerator(JitOptions options, IEnumerable<IInterceptorGenerator> generators)
        {
            this.options = options;
            this.generators = generators;
        }

        protected override IEnumerable<INotationGenerator> CreateNotationGenerators()
        {
            yield return new DefaultImplementNotationGenerator(options.FilterForDefaultImplement);
            yield return new ProxyNotationGenerator(generators);
        }

        protected override JitOptions CreateOptions()
        {
            return options;
        }
    }
}