using BenchmarkDotNet.Running;

namespace GenericContext
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<TestGenericContext>();
        }
    }
}