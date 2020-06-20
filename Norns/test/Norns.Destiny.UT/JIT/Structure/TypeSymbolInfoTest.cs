using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.JIT.Structure;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Norns.Destiny.UT.JIT.Structure
{
    internal class InternalClass
    { }

    public class PublicClass
    { }

    public abstract class AbstractPublicClass
    {
        public abstract void PublicMethod();

        public virtual void PublicVirtualMethod()
        {
        }

        protected abstract void ProtectedMethod();

        protected virtual void ProtectedVirtualMethod()
        {
        }

        internal abstract void InternalMethod();

        internal virtual void InternalVirtualMethod()
        {
        }

        protected internal abstract void ProtectedInternalMethod();

        protected internal virtual void ProtectedInternalVirtualMethod()
        {
        }

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
    }

    public class Test : AbstractPublicClass
    {
        public override void PublicMethod()
        {
            throw new NotImplementedException();
        }

        protected override void ProtectedMethod()
        {
            throw new NotImplementedException();
        }

        protected internal override void ProtectedInternalMethod()
        {
            throw new NotImplementedException();
        }

        internal override void InternalMethod()
        {
            throw new NotImplementedException();
        }
    }

    public static class StaticClass
    {
    }

    public class TypeSymbolInfoTest
    {
        #region Accessibility

        [Fact]
        public void AccessibilityWhenNestedClass()
        {
            var dict = typeof(AbstractPublicClass).GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .ToDictionary(i => i.Name, i => new TypeSymbolInfo(i as TypeInfo));
            Assert.Equal(AccessibilityInfo.Private, dict["PrivateClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.Protected, dict["ProtectedClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.Internal, dict["InternalClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.ProtectedOrInternal, dict["ProtectedInternalClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.ProtectedAndInternal, dict["ProtectedPrivateClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.Public, dict["PublicClass"].Accessibility);
        }

        [Fact]
        public void AccessibilityWhenNormalClass()
        {
            var dict = typeof(AbstractPublicClass).Assembly.GetTypes()
                   .ToDictionary(i => i.FullName, i => new TypeSymbolInfo(i));
            Assert.Equal(AccessibilityInfo.Internal, dict["Norns.Destiny.UT.JIT.Structure.InternalClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.Public, dict["Norns.Destiny.UT.JIT.Structure.PublicClass"].Accessibility);
        }

        #endregion Accessibility

        [Fact]
        public void WhenIsStatic()
        {
            Assert.True(new TypeSymbolInfo(typeof(StaticClass)).IsStatic);
            Assert.False(new TypeSymbolInfo(typeof(AbstractPublicClass)).IsStatic);
            Assert.False(new TypeSymbolInfo(typeof(InternalClass)).IsStatic);
            Assert.False(new TypeSymbolInfo(typeof(PublicClass)).IsStatic);
        }

        [Fact]
        public void WhenIsAnonymousType()
        {
            Assert.True(new TypeSymbolInfo(new { }.GetType()).IsAnonymousType);
            Assert.True(new TypeSymbolInfo(new { A = 9 }.GetType()).IsAnonymousType);
            Assert.False(new TypeSymbolInfo(typeof(StaticClass)).IsAnonymousType);
            Assert.False(new TypeSymbolInfo(typeof(PublicClass)).IsAnonymousType);
        }
    }
}