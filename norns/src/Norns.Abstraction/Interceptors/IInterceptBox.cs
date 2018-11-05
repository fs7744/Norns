namespace Norns.AOP.Interceptors
{
    public interface IInterceptBox
    {
        IInterceptor Interceptor { get; }

        InterceptPredicate Verifier { get; }
    }
}