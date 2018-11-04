using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Norns.DependencyInjection
{
    public class ServiceCollection : List<ServiceDescriptor>, IServiceCollection
    {
    }
}