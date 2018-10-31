using Norns.AOP.Attributes;
using Norns.Extensions.Reflection;
using System;
using Xunit;

namespace Norns.Test.Reflections
{
    [NoIntercept]
    [TestCustom("a", B = "b")]
    public class CustomAttributeTest
    {
        public class TestCustomAttribute : Attribute
        {
            public TestCustomAttribute(string a)
            {
                A = a;
            }

            public string A { get; }

            public string B { get; set; }
        }

        [Fact]
        public void WhenType()
        {
            Assert.False(typeof(CustomAttributeTest).GetReflector().IsDefined<FactAttribute>());
            Assert.True(typeof(CustomAttributeTest).GetReflector().IsDefined<NoInterceptAttribute>());
            var test = typeof(CustomAttributeTest).GetReflector().GetCustomAttribute<TestCustomAttribute>();
            Assert.Equal("a", test.A);
            Assert.Equal("b", test.B);
            Assert.Null(typeof(TestCustomAttribute).GetReflector().GetCustomAttribute<TestCustomAttribute>());
        }

        [Fact]
        [TestCustom("a2", B = "b3")]
        public void WhenMethod()
        {
            var method = typeof(CustomAttributeTest).GetMethod("WhenMethod");
            Assert.True(method.GetReflector().IsDefined<FactAttribute>());
            Assert.False(method.GetReflector().IsDefined<NoInterceptAttribute>());
            var test = method.GetReflector().GetCustomAttribute<TestCustomAttribute>();
            Assert.Equal("a2", test.A);
            Assert.Equal("b3", test.B);
            Assert.Null(typeof(CustomAttributeTest).GetMethod("WhenType").GetReflector().GetCustomAttribute<TestCustomAttribute>());
        }
    }
}