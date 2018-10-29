using System;

namespace DynamicContext
{
    public class TestInterceptor : ISyncInterceptor
    {
        public void OnInvokeSync(Context context, Action<Context> next)
        {
            int x = context.Parameters[0];
            int y = context.Parameters[1];
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
}