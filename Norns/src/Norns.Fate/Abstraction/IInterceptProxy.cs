using System;
using System.Collections.Generic;
using System.Text;

namespace Norns.Fate.Abstraction
{
    public interface IInterceptProxy
    {
        IServiceProvider Provider { get; set; }
    }
}
