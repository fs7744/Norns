using Norns.Destiny.AOP;
using Norns.Destiny.AOP.Notations;
using Norns.Destiny.Loom;
using Norns.Verthandi.Loom;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Verthandi.AOP
{
    public class AopSourceGenerator : AssemblyGenerator
    {
        protected readonly LoomOptions options;
        protected readonly IEnumerable<IInterceptorGenerator> generators;

        public AopSourceGenerator(LoomOptions options, IEnumerable<IInterceptorGenerator> generators)
        {
            this.options = options;
            this.generators = generators.ToArray();
        }

        protected override IEnumerable<INotationGenerator> CreateNotationGenerators()
        {
            yield return new DefaultImplementNotationGenerator(options.FilterForDefaultImplement);
            yield return new ProxyNotationGenerator(generators);
        }

        protected override LoomOptions CreateOptions()
        {
            return options;
        }
    }
}