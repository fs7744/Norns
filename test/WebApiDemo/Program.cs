using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Norns.Destiny.AOP;

namespace WebApiDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseVerthandiAop(new IInterceptorGenerator[] { new ConsoleCallMethodGenerator() })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}