using Norns.DependencyInjection;
using Xunit;

namespace Norns.Test.DependencyInjection
{
    public class SingletonTest
    {
        [Fact]
        public void SingletonWhenObject()
        {
            var service = new ServiceDefintions()
                .AddSingleton(this)
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void SingletonWhenTypeObject()
        {
            var service = new ServiceDefintions()
                .AddSingleton<SingletonTest, SingletonTest>(this)
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void SingletonWhenFactory()
        {
            var service = new ServiceDefintions()
                .AddSingleton(i => this)
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void SingletonWhenTypeFactory()
        {
            var service = new ServiceDefintions()
                .AddSingleton<SingletonTest, SingletonTest>(i => this)
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void SingletonWhenServiceType()
        {
            var service = new ServiceDefintions()
                .AddSingleton<SingletonTest, SingletonTest>()
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(SingletonTest));
            Assert.NotSame(this, result0);
            var result1 = service.GetService(typeof(SingletonTest));
            Assert.NotSame(this, result1);
            Assert.Same(result0, result1);
        }
    }
}