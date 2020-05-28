using BenchmarkDotNet.Running;

namespace DynamicContext
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<TestDynamicContext>();
        }
    }
}