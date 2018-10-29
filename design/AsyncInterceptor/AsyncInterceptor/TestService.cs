using System.Threading.Tasks;

namespace AsyncInterceptor
{
    public class TestService : ITestService
    {
        public int Sum(int x, int y)
        {
            var result = x;
            for (int i = 0; i < x; i++)
            {
                result += y;
            }
            return result;
        }

        public Task<int> SumAsync(int x, int y)
        {
            var result = x;
            for (int i = 0; i < x; i++)
            {
                result += y;
            }
            return Task.FromResult(result);
        }
    }
}