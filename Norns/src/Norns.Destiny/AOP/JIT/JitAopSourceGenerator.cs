using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.AOP.Notations;
using Norns.Destiny.JIT.Coder;
using System.Collections.Generic;

namespace Norns.Destiny.AOP.JIT
{
    public class JitAopSourceGenerator : JitAssemblyGenerator
    {
        private readonly JitOptions options;
        private readonly IEnumerable<IInterceptorGenerator> generators;

        public JitAopSourceGenerator(JitOptions options, IEnumerable<IInterceptorGenerator> generators)
        {
            this.options = options;
            this.generators = generators;
        }

        protected override IEnumerable<INotationGenerator> CreateNotationGenerators()
        {
            yield return new DefaultImplementNotationGenerator();
            yield return new ProxyNotationGenerator(generators);
        }

        protected override JitOptions CreateOptions()
        {
            return options;
        }
    }
}