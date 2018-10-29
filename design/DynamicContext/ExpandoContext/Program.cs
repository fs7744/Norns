using BenchmarkDotNet.Running;

namespace ExpandoContext
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<TestDynamicContext>();
        }
    }
}