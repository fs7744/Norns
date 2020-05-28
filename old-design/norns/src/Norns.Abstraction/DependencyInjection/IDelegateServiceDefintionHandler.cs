namespace Norns.DependencyInjection
{
    public interface IDelegateServiceDefintionHandler
    {
        int Order { get; }

        DelegateServiceDefintion Handle(DelegateServiceDefintion defintion);
    }
}