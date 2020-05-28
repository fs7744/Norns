using Microsoft.Extensions.DependencyInjection;
using Norns.Adapters.DependencyInjection.Attributes;
using Norns.Fate;
using Norns.UT.DependencyInjection;
using System;
using Xunit;

[assembly: ProxyMapping(typeof(ITestDependencyInjection), typeof(TestDependencyInjection), typeof(TestDependencyInjectionProxy))]

namespace Norns.UT.DependencyInjection
{
    public interface ITestDependencyInjection
    {
        int AddOne(int v);
    }

    [Proxy]
    public class TestDependencyInjection : ITestDependencyInjection
    {
        public int AddOne(int v)
        {
            return v + 1;
        }
    }

    public class TestDependencyInjectionProxy : ITestDependencyInjection
    {
        private readonly TestDependencyInjection implementation;

        public TestDependencyInjectionProxy(TestDependencyInjection implementation)
        {
            this.implementation = implementation;
        }

        public int AddOne(int v)
        {
            return implementation.AddOne(v) + 1;
        }
    }

    public class NornsDependencyInjectionExtensionsTest
    {
        private static (IServiceProvider, ITestDependencyInjection) Mock(Action<IServiceCollection> action)
        {
            var sc = new ServiceCollection();
            action(sc);
            var p = sc.BuildAopServiceProvider();
            var proxy = p.GetRequiredService<ITestDependencyInjection>();
            return (p, proxy);
        }

        [Fact]
        public void BuildAopServiceProviderWhenImplementationInstance()
        {
            var (serviceProvider, proxy) = Mock(sc => sc.AddSingleton<ITestDependencyInjection>(new TestDependencyInjection()));
            Assert.Equal(5, proxy.AddOne(3));
            Assert.Same(proxy, serviceProvider.GetRequiredService<ITestDependencyInjection>());
        }

        [Fact]
        public void BuildAopServiceProviderWhenAddTransientImplementationFactory()
        {
            var (serviceProvider, proxy) = Mock(sc => sc.AddTransient<ITestDependencyInjection>(i => new TestDependencyInjection()));
            Assert.Equal(10, proxy.AddOne(8));
            Assert.NotSame(proxy, serviceProvider.GetRequiredService<ITestDependencyInjection>());
        }

        [Fact]
        public void BuildAopServiceProviderWhenAddTransientImplementationFactoryHasType()
        {
            var (serviceProvider, proxy) = Mock(sc => sc.AddTransient<ITestDependencyInjection, TestDependencyInjection>(i => new TestDependencyInjection()));
            Assert.Equal(10, proxy.AddOne(8));
            Assert.NotSame(proxy, serviceProvider.GetRequiredService<ITestDependencyInjection>());
        }

        [Fact]
        public void BuildAopServiceProviderWhenAddScopedInterface()
        {
            var (serviceProvider, proxy) = Mock(sc => sc.AddScoped<ITestDependencyInjection, TestDependencyInjection>());
            Assert.Equal(11, proxy.AddOne(9));
            Assert.Same(proxy, serviceProvider.GetRequiredService<ITestDependencyInjection>());
        }
    }
}