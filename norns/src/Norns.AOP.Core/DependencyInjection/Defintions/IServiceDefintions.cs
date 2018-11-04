using System.Collections.Generic;

namespace Norns.DependencyInjection
{
    public interface IServiceDefintions : IEnumerable<ServiceDefintion>
    {
        void Add(ServiceDefintion serviceDefintion);
    }
}