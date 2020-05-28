using Norns.DependencyInjection;
using Xunit;

namespace Norns.Test.DependencyInjection
{
    public class NamedPropertyInjectorTest
    {
        public class TestPropertyInjectorClass
        {
            [FromDI(Named = "Test")]
            public ScopedTest Scoped { get; set; }
        }

        [Fact]
        public void WhenHasPropertyInjector()
        {
            var service = new ServiceDefintions()
                .AddScoped<TestPropertyInjectorClass>()
                .AddSingleton<ScopedTest>("Test")
                .AddSingleton<ScopedTest>()
                .BuildServiceProvider();

            var result0 = service.GetRequiredService<ScopedTest>("Test");
            var result1 = service.GetRequiredService<TestPropertyInjectorClass>();
            Assert.Same(result0, result1.Scoped);
        }

        [Fact]
        public void WhenHasPropertyInjectorButNoService()
        {
            var service = new ServiceDefintions()
                .AddSingleton<ScopedTest>()
                .AddScoped<TestPropertyInjectorClass>()
                .BuildServiceProvider();

            var result0 = service.GetService<ScopedTest>("Test");
            Assert.Null(result0);
            var result1 = service.GetRequiredService<TestPropertyInjectorClass>();
            Assert.Null(result1.Scoped);
        }
    }
}