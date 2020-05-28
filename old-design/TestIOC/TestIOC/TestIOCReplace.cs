using AsyncInterceptor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestIOC
{
    public class TestIOCReplace
    {
        public void ReplaceProxy()
        {
            var service = new ServiceCollection()
                .AddTransient<ITestService, TestService>();

            service.AddAop();

            var provider = service.BuildServiceProvider();

            var d = provider.GetRequiredService<ITestService>()
                .Sum(6, 6);
        }
    }
}
