using System.Dynamic;

namespace ExpandoContext
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

        private void SumReal(Context context)
        {
            context.Info.Result = realService.Sum(context.Info.x, context.Info.y);
        }

        public int Sum(int x, int y)
        {
            var context = new Context()
            {
                Info = new ExpandoObject()
            };
            context.Info.x = x;
            context.Info.y = y;
            interceptor.OnInvokeSync(context, SumReal);
            return context.Info.Result;
        }

        private void SumReal2(Context context)
        {
            realService.Sum();
        }

        public void Sum()
        {
            var context = new Context()
            {
            };
            interceptor.OnInvokeSync(context, SumReal2);
        }
    }
}