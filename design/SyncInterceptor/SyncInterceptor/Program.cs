using BenchmarkDotNet.Running;

namespace SyncInterceptor
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<TestSyncInterceptor>();
        }
    }
}