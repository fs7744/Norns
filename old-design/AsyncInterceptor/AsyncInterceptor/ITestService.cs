using System.Threading.Tasks;

namespace AsyncInterceptor
{
    public interface ITestService
    {
        int Sum(int x, int y);

        Task<int> SumAsync(int x, int y);
    }
}