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

    public sealed class SealedClass
    {
    }

    public interface IA
    {
    }

    public interface IA<out T> where T : AbstractPublicClass, new()
    {
        public T A() => default;
    }

    public interface IB<in T>
    {
    }

    public struct A
    { }

    public enum B
    {
        One,
        Two
    }

    public class GenericClass<T, Y> where T : class, new() where Y : struct, Enum
    {
        public (T, Y) A() => default;
    }

    public class FieldTest
    {
        public const int A = 3;
        internal static readonly string B = "3";
        protected volatile string C;
        protected internal long D;
        private protected long E;
        private long F;
    }

    public unsafe struct StructFieldTest
    {
        internal fixed char name[30];
    }

    public class TypeSymbolInfoTest
    {
        #region Accessibility

        [Fact]
        public void AccessibilityWhenNestedClass()
        {
            var dict = typeof(AbstractPublicClass).GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .ToDictionary(i => i.Name, i => new TypeSymbolInfo(i as TypeInfo));
            Assert.Equal("Norns.Destiny.UT.JIT.Structure.AbstractPublicClass+PrivateClass", dict["PrivateClass"].FullName);
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
            Assert.True(new TypeSymbolInfo(typeof(StaticClass)).IsSealed);
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

        [Fact]
        public void WhenNameAndNamespace()
        {
            var type = new TypeSymbolInfo(typeof(StaticClass));
            Assert.Equal("StaticClass", type.Name);
            Assert.Equal("Norns.Destiny.UT.JIT.Structure", type.Namespace);
        }

        [Fact]
        public void WhenIsSealed()
        {
            Assert.True(new TypeSymbolInfo(typeof(SealedClass)).IsSealed);
            Assert.False(new TypeSymbolInfo(typeof(PublicClass)).IsSealed);
        }

        [Fact]
        public void WhenIsAbstract()
        {
            Assert.True(new TypeSymbolInfo(typeof(AbstractPublicClass)).IsAbstract);
            Assert.False(new TypeSymbolInfo(typeof(PublicClass)).IsAbstract);
        }

        [Fact]
        public void WhenIsValueType()
        {
            Assert.True(new TypeSymbolInfo(typeof(int)).IsValueType);
            Assert.True(new TypeSymbolInfo(typeof(IntPtr)).IsValueType);
            Assert.False(new TypeSymbolInfo(typeof(PublicClass)).IsValueType);
        }

        #region IsGenericType

        [Fact]
        public void IsGenericTypeWhenIA()
        {
            var iaTypeDefinition = new TypeSymbolInfo(typeof(IA<>));
            Assert.True(iaTypeDefinition.IsGenericType);
            Assert.Empty(iaTypeDefinition.TypeArguments);
            Assert.Single(iaTypeDefinition.TypeParameters);
            var tp = iaTypeDefinition.TypeParameters.First();
            Assert.Equal(0, tp.Ordinal);
            Assert.Equal("T", tp.Name);
            Assert.Equal(VarianceKindInfo.Out, tp.VarianceKind);
            Assert.True(tp.HasConstructorConstraint);
            Assert.False(tp.HasReferenceTypeConstraint);
            Assert.False(tp.HasValueTypeConstraint);
            Assert.Single(tp.ConstraintTypes);
            var tpc = tp.ConstraintTypes.First();
            Assert.Equal(nameof(AbstractPublicClass), tpc.Name);
            Assert.False(new TypeSymbolInfo(typeof(PublicClass)).IsGenericType);
        }

        [Fact]
        public void IsGenericTypeWhenIB()
        {
            var iaTypeDefinition = new TypeSymbolInfo(typeof(IB<Test>));
            Assert.True(iaTypeDefinition.IsGenericType);
            Assert.True(iaTypeDefinition.IsInterface);
            Assert.Single(iaTypeDefinition.TypeArguments);
            var ta = iaTypeDefinition.TypeArguments.First();
            Assert.Equal(nameof(Test), ta.Name);
            Assert.Single(iaTypeDefinition.TypeParameters);
            var tp = iaTypeDefinition.TypeParameters.First();
            Assert.Equal(0, tp.Ordinal);
            Assert.Equal("T", tp.Name);
            Assert.Equal(VarianceKindInfo.In, tp.VarianceKind);
            Assert.False(tp.HasConstructorConstraint);
            Assert.False(tp.HasReferenceTypeConstraint);
            Assert.False(tp.HasValueTypeConstraint);
            Assert.Empty(tp.ConstraintTypes);
        }

        [Fact]
        public void IsGenericTypeWhenGenericClass()
        {
            var iaTypeDefinition = new TypeSymbolInfo(typeof(GenericClass<Test, B>));
            Assert.True(iaTypeDefinition.IsGenericType);
            Assert.True(iaTypeDefinition.IsClass);
            Assert.Equal(2, iaTypeDefinition.TypeArguments.Length);
            var ta = iaTypeDefinition.TypeArguments.First();
            Assert.Equal(nameof(Test), ta.Name);
            Assert.Equal(2, iaTypeDefinition.TypeParameters.Length);
            var tp = iaTypeDefinition.TypeParameters.First();
            Assert.Equal(0, tp.Ordinal);
            Assert.Equal("T", tp.Name);
            Assert.Equal(VarianceKindInfo.None, tp.VarianceKind);
            Assert.True(tp.HasConstructorConstraint);
            Assert.True(tp.HasReferenceTypeConstraint);
            Assert.False(tp.HasValueTypeConstraint);
            Assert.Empty(tp.ConstraintTypes);

            ta = iaTypeDefinition.TypeArguments[1];
            Assert.Equal(nameof(B), ta.Name);
            Assert.Equal(2, iaTypeDefinition.TypeParameters.Length);
            tp = iaTypeDefinition.TypeParameters[1];
            Assert.Equal(1, tp.Ordinal);
            Assert.Equal("Y", tp.Name);
            Assert.Equal(VarianceKindInfo.None, tp.VarianceKind);
            Assert.True(tp.HasConstructorConstraint);
            Assert.False(tp.HasReferenceTypeConstraint);
            Assert.True(tp.HasValueTypeConstraint);
            Assert.Single(tp.ConstraintTypes);
            var tpc = tp.ConstraintTypes.First();
            Assert.Equal(nameof(Enum), tpc.Name);
        }

        #endregion IsGenericType

        [Fact]
        public void WhenBaseType()
        {
            Assert.Null(new TypeSymbolInfo(typeof(object)).BaseType);
            Assert.Equal(nameof(ValueType), new TypeSymbolInfo(typeof(int)).BaseType.Name);
            Assert.Equal(nameof(Object), new TypeSymbolInfo(typeof(AbstractPublicClass)).BaseType.Name);
            Assert.Equal(nameof(AbstractPublicClass), new TypeSymbolInfo(typeof(Test)).BaseType.Name);
            Assert.Equal(nameof(ValueType), new TypeSymbolInfo(typeof(A)).BaseType.Name);
        }

        [Fact]
        public void WhenInterfaces()
        {
            var interfaces = new TypeSymbolInfo(typeof(object)).GetInterfaces();
            Assert.Empty(interfaces);
            interfaces = new TypeSymbolInfo(typeof(int)).GetInterfaces();
            Assert.Contains(nameof(IComparable), interfaces.Select(i => i.Name));
            interfaces = new TypeSymbolInfo(typeof(Test)).GetInterfaces();
            Assert.Empty(interfaces);
            interfaces = new TypeSymbolInfo(typeof(A)).GetInterfaces();
            Assert.Empty(interfaces);
        }

        [Fact]
        public void WhenFields()
        {
            var fields = new TypeSymbolInfo(typeof(FieldTest)).GetMembers().Select(i => i as IFieldSymbolInfo)
                .Where(i => i != null)
                .ToArray();
            Assert.Equal(6, fields.Length);
        }
    }
}