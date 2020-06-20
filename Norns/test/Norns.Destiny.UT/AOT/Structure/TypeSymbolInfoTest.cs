using Norns.Destiny.Abstraction.Structure;
using Xunit;

namespace Norns.Destiny.UT.AOT.Structure
{
    public class TypeSymbolInfoTest
    {
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
    }
}