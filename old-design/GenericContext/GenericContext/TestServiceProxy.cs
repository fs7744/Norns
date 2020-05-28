using System;

namespace GenericContext
{
    public class TestServiceProxy : ITestService
    {
        private readonly ITestService realService;
        private readonly ISyncInterceptor interceptor;

        public TestServiceProxy(ITestService realService, ISyncInterceptor interceptor)
        {
            this.realService = realService;
            this.interceptor = interceptor;
        }

        private void SumReal(Context<Tuple<int, int>, int> context)
        {
            context.Result = realService.Sum(context.Parameters.Item1, context.Parameters.Item2);
        }

        public int Sum(int x, int y)
        {
            var context = new Context<Tuple<int,int>, int>()
            {
                Parameters = Tuple.Create(x, y)
            };
            interceptor.OnInvokeSync(context, SumReal);
            return context.Result;
        }

        private void SumRealNotTuple(Context<Tuple<int, int>, int> context)
        {
            context.Result = realService.Sum(context.X, context.Y);
        }

        public int SumNotTuple(int x, int y)
        {
            var context = new Context<Tuple<int, int>, int>()
            {
                X = x,
                Y = y
            };
            interceptor.OnInvokeSync(context, SumRealNotTuple);
            return context.Result;
        }

        private void SumReal2(Context context)
        {
            realService.Sum();
        }

        public void Sum()
        {
            var context = new Context<Tuple<int, int>, int>();
            interceptor.OnInvokeSync(context, SumReal2);
        }
    }
}