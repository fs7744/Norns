using Norns.Destiny.RuntimeSymbol;
using Norns.Destiny.UT.Skuld;
using System;
using Xunit;

namespace Norns.Destiny.UT.Structure
{
    public class SpecialTypeTest
    {
        private static void CompareType(Type type)
        {
            var aot = SkuldTest.GetTypeByMetadataName(type.FullName);
            var jit = type.GetSymbolInfo();
            Assert.NotEqual(aot.Origin, jit.Origin);
            Assert.Equal(aot.Name, jit.Name);
            Assert.Equal(aot.FullName, jit.FullName);
            Assert.Equal(aot.Namespace, jit.Namespace);
            Assert.Equal(aot.IsAbstract, jit.IsAbstract);
            Assert.Equal(aot.IsSealed, jit.IsSealed);
            Assert.Equal(aot.IsValueType, jit.IsValueType);
            Assert.Equal(aot.IsGenericType, jit.IsGenericType);
            Assert.Equal(aot.IsAnonymousType, jit.IsAnonymousType);
            Assert.Equal(aot.IsClass, jit.IsClass);
            Assert.Equal(aot.IsInterface, jit.IsInterface);
            Assert.Equal(aot.IsStatic, jit.IsStatic);
            Assert.Equal(aot.Accessibility, jit.Accessibility);
            Assert.Equal(aot.GenericDefinitionName, jit.GenericDefinitionName);
            Assert.Equal(aot.TypeArguments.IsEmpty, jit.TypeArguments.IsEmpty);
            Assert.Equal(aot.TypeParameters.IsEmpty, jit.TypeParameters.IsEmpty);
            Assert.Equal(aot.BaseType, jit.BaseType);
            Assert.Equal(aot.Interfaces.Count, jit.Interfaces.Count);
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(char))]
        [InlineData(typeof(short))]
        [InlineData(typeof(ushort))]
        [InlineData(typeof(int))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(long))]
        [InlineData(typeof(ulong))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(string))]
        public void TypeSymbolInfoWhenSpecialTypeShouldEq(Type type)
        {
            CompareType(type);
        }

        [Fact]
        public void VoidTypeShouldEq()
        {
            CompareType(Type.GetType("System.Void"));
        }



        [Fact]
        public void Test()
        {
            var b = Verthandi.AOP.VerthandiTest.Generate(typeof(System.Array));
        }
    }
}