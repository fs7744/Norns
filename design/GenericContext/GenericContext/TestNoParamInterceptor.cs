using System;

namespace GenericContext
{
    public class TestNoParamInterceptor : ISyncInterceptor
    {
        public void OnInvokeSync(Context<Tuple<int, int>, int> context, Action<Context<Tuple<int, int>, int>> next)
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