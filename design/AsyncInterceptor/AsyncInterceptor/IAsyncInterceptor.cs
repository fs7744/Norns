using System;
using System.Threading.Tasks;

namespace AsyncInterceptor
{
    public interface IAsyncInterceptor
    {
        Task OnInvokeAsync(Context context, Func<Context, Task> next);
    }

    public interface IInterceptor : IAsyncInterceptor, ISyncInterceptor
    {
    }
}