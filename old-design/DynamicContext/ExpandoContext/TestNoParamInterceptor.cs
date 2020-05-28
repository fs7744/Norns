using System;

namespace ExpandoContext
{
    public class TestNoParamInterceptor : ISyncInterceptor
    {
        public void OnInvokeSync(Context context, Action<Context> next)
        {
            var result = 40;
            try
            {
                result += 4;
                next(context);
            }
            finally
            {
                result -= 4;
                result -= 4;
            }
        }
    }
}