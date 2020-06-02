using System;
using System.Collections.Generic;
using System.Text;

namespace Norns.DestinyLoom
{
    public interface IInterceptorGenerator
    {
        IEnumerable<string> BeforeMethod(ProxyMethodGeneratorContext context);
        IEnumerable<string> AfterMethod(ProxyMethodGeneratorContext context);
    }
}
