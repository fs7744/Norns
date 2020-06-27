using Microsoft.Extensions.DependencyInjection;

namespace Norns.Benchmark
{
    public interface IC
    {
        int AddOne(int v);

        public int AddOne2(int v)
        {
            return v + 1;
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var p = new ServiceCollection()
                //.AddSingleton<IC, DsProxy>() sd
                .AddDestinyInterface<IC>(ServiceLifetime.Scoped)
                .BuildAopServiceProvider()
                .GetRequiredService<IC>();

            p.AddOne2(1);
        }
    }
}