using System.Threading.Tasks;
using TestIOC;

namespace AsyncInterceptor
{
    [Aop]
    public interface ITestService
    {
        int Sum(int x, int y);

        Task<int> SumAsync(int x, int y);
    }
}