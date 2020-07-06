using Norns.Destiny.AOP;
using Norns.Destiny.AOP.Notations;
using Norns.Destiny.Loom;
using Norns.Verthandi.AOP;
using Norns.Verthandi.Loom;
using System.Collections.Generic;

namespace Norns.Destiny.UT.Verthandi.AOP
{
    public class OnlyDefaultImplementNotationGenerator : AopSourceGenerator
    {
        public OnlyDefaultImplementNotationGenerator(LoomOptions options, IEnumerable<IInterceptorGenerator> generators) : base(options, generators)
        {
        }

        protected override IEnumerable<INotationGenerator> CreateNotationGenerators()
        {
            yield return new DefaultImplementNotationGenerator(options.FilterForDefaultImplement);
        }
    }
}