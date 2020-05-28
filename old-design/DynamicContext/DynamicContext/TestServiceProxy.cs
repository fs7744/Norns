namespace DynamicContext
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
            int x = context.Parameters[0];
            int y = context.Parameters[1];
            context.Result = realService.Sum(x, y);
        }

        public int Sum(int x, int y)
        {
            var context = new Context()
            {
                Parameters = new dynamic[] { x, y }
            };
            interceptor.OnInvokeSync(context, SumReal);
            return context.Result;
        }

        private void SumReal2(Context context)
        {
            realService.Sum();
        }

        public void Sum()
        {
            var context = new Context()
            {
                Parameters = new dynamic[0]
            };
            interceptor.OnInvokeSync(context, SumReal2);
        }
    }
}