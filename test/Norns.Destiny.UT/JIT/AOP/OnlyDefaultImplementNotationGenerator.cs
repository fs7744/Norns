using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.AOP;
using Norns.Destiny.AOP.Notations;
using Norns.Destiny.JIT.AOP;
using Norns.Destiny.JIT.Coder;
using System.Collections.Generic;

namespace Norns.Destiny.UT.JIT.AOP
{
    public class OnlyDefaultImplementNotationGenerator : JitAopSourceGenerator
    {
        public OnlyDefaultImplementNotationGenerator(JitOptions options, IEnumerable<IInterceptorGenerator> generators) : base(options, generators)
        {
        }

        protected override IEnumerable<INotationGenerator> CreateNotationGenerators()
        {
            yield return new DefaultImplementNotationGenerator(options.FilterForDefaultImplement);
        }
    }
}