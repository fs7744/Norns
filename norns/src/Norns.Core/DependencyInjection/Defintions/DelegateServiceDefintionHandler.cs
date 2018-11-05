using Norns.AOP.Attributes;
using System.Linq;

namespace Norns.DependencyInjection
{
    [NoIntercept]
    internal class DelegateServiceDefintionHandler : IDelegateServiceDefintionHandler
    {
        private readonly IDelegateServiceDefintionHandler[] handlers;

        public int Order => 0;

        public DelegateServiceDefintionHandler(IServiceDefintions services)
        {
            handlers = services.Select(i =>
            {
                if (i is DelegateServiceDefintion defintion
                    && defintion.ServiceType == typeof(IDelegateServiceDefintionHandler))
                {
                    return (IDelegateServiceDefintionHandler)defintion.ImplementationFactory(null);
                }
                else
                {
                    return null;
                }
            })
            .Where(i => i != null)
            .OrderBy(i => i.Order)
            .ToArray();
        }

        public DelegateServiceDefintion Handle(DelegateServiceDefintion defintion)
        {
            foreach (var handler in handlers)
            {
                defintion = handler.Handle(defintion);
            }
            return defintion;
        }
    }
}