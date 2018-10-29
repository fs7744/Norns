using BenchmarkDotNet.Running;
using System;

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