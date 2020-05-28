using AsyncInterceptor;
using System;
using System.Threading.Tasks;

namespace TestIOC
{
    public class AopAttribute : Attribute, IInterceptor
    {
        public async Task OnInvokeAsync(Context context, Func<Context, Task> next)
        {
            Console.WriteLine("AopBegin");
            await next(context);
            Console.WriteLine("AopEnd");
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