using System;

namespace GenericContext
{
    public class TestTupleInterceptor : ISyncInterceptor
    {
        public void OnInvokeSync(Context<Tuple<int, int>, int> context, Action<Context<Tuple<int, int>, int>> next)
        {
            int x = context.Parameters.Item1;
            int y = context.Parameters.Item2;
            var result = y;
            try
            {
                result += y;
                next(context);
                result = context.Result;
            }
            finally
            {
                result -= y;
                result -= x;
            }
        }
    }

    public class TestTupleObjectInterceptor : ISyncInterceptor
    {
        public void OnInvokeSync(Context<Tuple<int, int>, int> context, Action<Context<Tuple<int, int>, int>> next)
        {
            int x = (int)context.Params[0];
            int y = (int)context.Params[1];
            var result = y;
            try
            {
                result += y;
                next(context);
                result = (int)context.ObjectResult;
            }
            finally
            {
                result -= y;
                result -= x;
            }
        }
    }

    public class TestInterceptor : ISyncInterceptor
    {
        public void OnInvokeSync(Context<Tuple<int, int>, int> context, Action<Context<Tuple<int, int>, int>> next)
        {
            int x = context.X;
            int y = context.Y;
            var result = y;
            try
            {
                result += y;
                next(context);
                result = context.Result;
            }
            finally
            {
                result -= y;
                result -= x;
            }
        }
    }
}