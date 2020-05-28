using System.Threading.Tasks;

namespace Norns.Fate.Abstraction
{
    public delegate Task InterceptAsync(FateContext context);

    public delegate void Intercept(FateContext context);

    public interface IInterceptor
    {
        void Invoke(FateContext context, Intercept next);

        Task InvokeAsync(FateContext context, InterceptAsync next);

        //public virtual void Invoke(FateContext context, Intercept next)
        //{
        //    Task InvokeNextAsync(FateContext c)
        //    {
        //        next(c);
        //        return Task.CompletedTask;
        //    }

        //    InvokeAsync(context, InvokeNextAsync).ConfigureAwait(false).GetAwaiter().GetResult();
        //}
    }
}