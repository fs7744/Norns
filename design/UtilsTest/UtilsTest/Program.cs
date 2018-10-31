using BenchmarkDotNet.Running;

namespace UtilsTest
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<TestUtilsTest>();
        }
    }
}