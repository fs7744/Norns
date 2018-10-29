namespace SyncInterceptor
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
            var x = (int)context.Parameters[0];
            var y = (int)context.Parameters[1];
            context.Result = realService.Sum(x, y);
        }

        public int Sum(int x, int y)
        {
            var context = new Context()
            {
                Parameters = new object[] { x, y }
            };
            interceptor.OnInvokeSync(context, SumReal);
            return (int)context.Result;
        }

        private void SumReal2(Context context)
        {
            realService.Sum();
        }

        public void Sum()
        {
            var context = new Context()
            {
                Parameters = new object[0]
            };
            interceptor.OnInvokeSync(context, SumReal2);
        }
    }
}