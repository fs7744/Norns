namespace ExpandoContext
{
    public class TestServiceHandWriting : ITestService
    {
        private readonly ITestService realService;

        public TestServiceHandWriting(ITestService realService)
        {
            this.realService = realService;
        }

        public int Sum(int x, int y)
        {
            var result = x;
            try
            {
                result += y;
                result = realService.Sum(x, y);
            }
            finally
            {
                result -= y;
                result -= x;
            }
            return result;
        }

        public void Sum()
        {
            var result = 40;
            try
            {
                result += 4;
                realService.Sum();
            }
            finally
            {
                result -= 4;
                result -= 4;
            }
        }
    }
}