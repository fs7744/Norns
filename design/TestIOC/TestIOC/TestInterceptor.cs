using System;
using System.Threading.Tasks;

namespace AsyncInterceptor
{
    public class TestInterceptor : IInterceptor
    {
        public async Task OnInvokeAsync(Context context, Func<Context, Task> next)
        {
            var x = (int)context.Parameters[0];
            var y = (int)context.Parameters[1];
            var result = y;
            try
            {
                result += y;
                await next(context);
            }
            finally
            {
                result -= y;
                result -= x;
            }
        }

        public void OnInvokeSync(Context context, Action<Context> next)
        {
            var x = (int)context.Parameters[0];
            var y = (int)context.Parameters[1];
            var result = y;
            try
            {
                result += y;
                next(context);
            }
            finally
            {
                result -= y;
                result -= x;
            }
        }
    }

    public class TestInterceptor_SyncBaseOnAsync : IInterceptor
    {
        public async Task OnInvokeAsync(Context context, Func<Context, Task> next)
        {
            var x = (int)context.Parameters[0];
            var y = (int)context.Parameters[1];
            var result = y;
            try
            {
                result += y;
                await next(context);
            }
            finally
            {
                result -= y;
                result -= x;
            }
        }

        public void OnInvokeSync(Context context, Action<Context> next)
        {
            OnInvokeAsync(context, c =>
            {
                next(c);
                return Task.CompletedTask;
            }).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}