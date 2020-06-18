using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.JIT.Coder;

namespace Norns.Destiny.AOP.JIT
{
    public class JitSourceGenerator : JitAssemblyGenerator
    {
        private readonly JitOptions options;

        public JitSourceGenerator(JitOptions options)
        {
            this.options = options;
        }

        protected override INotationGenerator CreateNotationGenerator()
        {
            return new ProxyNotationGenerator();
        }

        protected override JitOptions CreateOptions()
        {
            return options;
        }
    }
}