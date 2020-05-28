using Norns.DependencyInjection;
using Xunit;

namespace Norns.Test.DependencyInjection
{
    public class PropertyInjectorTest
    {
        public class TestPropertyInjectorClass
        {
            [FromDI]
            public ScopedTest Scoped { get; set; }
        }

        [Fact]
        public void WhenHasPropertyInjector()
        {
            var service = new ServiceDefintions()
                .AddScoped<TestPropertyInjectorClass>()
                .AddSingleton<ScopedTest>()
                .BuildServiceProvider();

            var result0 = service.GetRequiredService<ScopedTest>();
            var result1 = service.GetRequiredService<TestPropertyInjectorClass>();
            Assert.Same(result0, result1.Scoped);
        }

        [Fact]
        public void WhenHasPropertyInjectorButNoService()
        {
            var service = new ServiceDefintions()
                .AddScoped<TestPropertyInjectorClass>()
                .BuildServiceProvider();

            var result0 = service.GetService<ScopedTest>();
            Assert.Null(result0);
            var result1 = service.GetRequiredService<TestPropertyInjectorClass>();
            Assert.Null(result1.Scoped);
        }
    }
}