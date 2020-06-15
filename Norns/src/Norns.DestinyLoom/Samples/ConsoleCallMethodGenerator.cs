using System.Collections.Generic;
using System.Linq;

namespace Norns.DestinyLoom.Samples
{
    public class ConsoleCallMethodGenerator : AbstractInterceptorGenerator
    {
        public override IEnumerable<string> BeforeMethod(ProxyMethodGeneratorContext context)
        {
            if (!context.Method.Parameters.IsEmpty)
            {
                yield return $"System.Console.WriteLine($\"Call Method {context.Method.Name} at {{System.DateTime.Now.ToString(\"yyyy-MM-dd HH:mm:ss.fff\")}} {context.Method.Parameters[0].Type.ToDisplayString()} {context.Method.Parameters[0].Name} = {{{context.Method.Parameters[0].Name}}}";
                foreach (var item in context.Method.Parameters.Skip(1))
                {
                    yield return $", {item.ToDisplayString()} {item.Name} = {{{item.Name}}}";
                }
                yield return "\");";
            }
        }

        public override IEnumerable<string> AfterMethod(ProxyMethodGeneratorContext context)
        {
            if (context.HasReturnValue)
            {
                yield return $"System.Console.WriteLine($\"return {{{context.ReturnValueParameterName}}} at {{System.DateTime.Now.ToString(\"yyyy-MM-dd HH:mm:ss.fff\")}}\");";
            }
        }
    }
}