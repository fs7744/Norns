using BenchmarkDotNet.Running;

namespace AsyncInterceptor
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<TestAsyncInterceptor>();
        }
    }
}