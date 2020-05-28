using BenchmarkDotNet.Running;
using Norns.AOP.Configuration;
using Norns.AOP.Interceptors;
using Norns.Core.AOP.Configuration;
using Norns.DependencyInjection;
using System;

namespace TestFuncToDll
{
    [Norns.AOP.Attributes.NoIntercept]
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                new Test().ProxyJustCall();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("No");
            BenchmarkRunner.Run<Test>();
        }
    }
}