using System;

namespace SyncInterceptor
{
    public class TestInterceptor : ISyncInterceptor
    {
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
}