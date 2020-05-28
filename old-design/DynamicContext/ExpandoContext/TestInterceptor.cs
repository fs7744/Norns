using System;

namespace ExpandoContext
{
    public class TestInterceptor : ISyncInterceptor
    {
        public void OnInvokeSync(Context context, Action<Context> next)
        {
            int x = context.Info.x;
            int y = context.Info.y;
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
                context.Info.Result = result;
            }
        }
    }
}