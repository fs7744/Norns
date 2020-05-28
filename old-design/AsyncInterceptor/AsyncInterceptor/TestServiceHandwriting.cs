using System.Threading.Tasks;

namespace AsyncInterceptor
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
                realService.Sum(x, y);
            }
            finally
            {
                result -= y;
                result -= x;
            }
            return result;
        }

        public async Task<int> SumAsync(int x, int y)
        {
            var result = x;
            try
            {
                result += y;
                await realService.SumAsync(x, y);
            }
            finally
            {
                result -= y;
                result -= x;
            }
            return result;
        }
    }
}