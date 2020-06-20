using Norns.Destiny.Abstraction.Structure;
using Xunit;

namespace Norns.Destiny.UT.AOT.Structure
{
    public class TypeSymbolInfoTest
    {
        #region Accessibility

        [Fact]
        public void AccessibilityWhenNestedClass()
        {
            var code = @"public abstract class AbstractPublicClass
    {
        private class PrivateClass
        { }

        private protected class ProtectedPrivateClass
        { }

        protected class ProtectedClass
        { }

        internal class InternalClass
        { }

        protected internal class ProtectedInternalClass
        { }

        public class PublicClass
        { }
    }";
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.Equal(AccessibilityInfo.Private, types["PrivateClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.Protected, types["ProtectedClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.Internal, types["InternalClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.ProtectedOrInternal, types["ProtectedInternalClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.ProtectedAndInternal, types["ProtectedPrivateClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.Public, types["PublicClass"].Accessibility);
        }

        [Fact]
        public void AccessibilityWhenNormalClass()
        {
            var code = "public class PublicClass {} internal class InternalClass{}";
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.Equal(AccessibilityInfo.Internal, types["InternalClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.Public, types["PublicClass"].Accessibility);
        }

        #endregion Accessibility

        [Fact]
        public void WhenIsStatic()
        {
            var code = "public static class StaticClass {} class NonStaticClass{}";
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.True(types["StaticClass"].IsStatic);
            Assert.False(types["NonStaticClass"].IsStatic);
        }

        [Fact]
        public void WhenIsAnonymousType()
        {
            var code = "public static class StaticClass { void A() { var a = new {}; var b = new { A = 9}; } } ";
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.False(types["StaticClass"].IsAnonymousType);
        }

        [Fact]
        public void WhenNameAndNamespace()
        {
            var code = "public class ADD { } ";
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.Equal("Norns.Destiny.UT.AOT.Generated", types["ADD"].Namespace);
        }

        [Fact]
        public void WhenIsSealed()
        {
            var code = "public sealed class SealedClass { } public class NonSealedClass { }";
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.True(types["SealedClass"].IsSealed);
            Assert.False(types["NonSealedClass"].IsSealed);
        }
        
        [Fact]
        public void WhenIsAbstract()
        {
            var code = "public abstract class AbstractClass { } public class NonAbstractClass { }";
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.True(types["AbstractClass"].IsAbstract);
            Assert.False(types["NonAbstractClass"].IsAbstract);
        }

        [Fact]
        public void WhenIsValueType()
        {
            var code = "public struct MyStruct { } public class ClassT { }";
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.True(types["MyStruct"].IsValueType);
            Assert.False(types["ClassT"].IsValueType);
        }

        [Fact]
        public void WhenIsGenericType()
        {
            var code = @"public class ClassT { }

public class GenericClass<T> where T : ClassT, new()
{
    public T A() => default;
}";
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.True(types["GenericClass"].IsGenericType);
            Assert.True(types["GenericClass"].IsClass);
            Assert.False(types["ClassT"].IsGenericType);
        }
    }
}