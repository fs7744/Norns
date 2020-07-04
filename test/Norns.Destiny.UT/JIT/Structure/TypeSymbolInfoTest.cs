using Norns.Destiny.Abstraction.Structure;
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

        [Collection("a")]
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
                .ToDictionary(i => i.Name, i => (i as TypeInfo).GetSymbolInfo());
            Assert.Equal("Norns.Destiny.UT.JIT.Structure.AbstractPublicClass.PrivateClass", dict["PrivateClass"].FullName);
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
                   .ToDictionary(i => i.FullName, i => i.GetSymbolInfo());
            Assert.Equal(AccessibilityInfo.Internal, dict["Norns.Destiny.UT.JIT.Structure.InternalClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.Public, dict["Norns.Destiny.UT.JIT.Structure.PublicClass"].Accessibility);
        }

        #endregion Accessibility

        [Fact]
        public void WhenIsStatic()
        {
            Assert.True(typeof(StaticClass).GetSymbolInfo().IsStatic);
            Assert.True(typeof(StaticClass).GetSymbolInfo().IsSealed);
            Assert.False(typeof(AbstractPublicClass).GetSymbolInfo().IsStatic);
            Assert.False(typeof(InternalClass).GetSymbolInfo().IsStatic);
            Assert.False(typeof(PublicClass).GetSymbolInfo().IsStatic);
        }

        [Fact]
        public void WhenIsAnonymousType()
        {
            Assert.True(new { }.GetType().GetSymbolInfo().IsAnonymousType);
            Assert.True(new { A = 9 }.GetType().GetSymbolInfo().IsAnonymousType);
            Assert.False(typeof(StaticClass).GetSymbolInfo().IsAnonymousType);
            Assert.False(typeof(PublicClass).GetSymbolInfo().IsAnonymousType);
        }

        [Fact]
        public void WhenNameAndNamespace()
        {
            var type = typeof(StaticClass).GetSymbolInfo();
            Assert.Equal("StaticClass", type.Name);
            Assert.Equal("Norns.Destiny.UT.JIT.Structure", type.Namespace);
        }

        [Fact]
        public void WhenIsSealed()
        {
            Assert.True(typeof(SealedClass).GetSymbolInfo().IsSealed);
            Assert.False(typeof(PublicClass).GetSymbolInfo().IsSealed);
        }

        [Fact]
        public void WhenIsAbstract()
        {
            Assert.True(typeof(AbstractPublicClass).GetSymbolInfo().IsAbstract);
            Assert.False(typeof(PublicClass).GetSymbolInfo().IsAbstract);
        }

        [Fact]
        public void WhenIsValueType()
        {
            Assert.True(typeof(int).GetSymbolInfo().IsValueType);
            Assert.True(typeof(IntPtr).GetSymbolInfo().IsValueType);
            Assert.False(typeof(PublicClass).GetSymbolInfo().IsValueType);
        }

        #region IsGenericType

        [Fact]
        public void IsGenericTypeWhenIA()
        {
            var iaTypeDefinition = typeof(IA<>).GetSymbolInfo();
            Assert.True(iaTypeDefinition.IsGenericType);
            Assert.Empty(iaTypeDefinition.TypeArguments);
            Assert.Single(iaTypeDefinition.TypeParameters);
            var tp = iaTypeDefinition.TypeParameters.First();
            Assert.Equal(0, tp.Ordinal);
            Assert.Equal("T", tp.Name);
            Assert.Equal(RefKindInfo.Out, tp.RefKind);
            Assert.True(tp.HasConstructorConstraint);
            Assert.False(tp.HasReferenceTypeConstraint);
            Assert.False(tp.HasValueTypeConstraint);
            Assert.Single(tp.ConstraintTypes);
            var tpc = tp.ConstraintTypes.First();
            Assert.Equal(nameof(AbstractPublicClass), tpc.Name);
            Assert.False(typeof(PublicClass).GetSymbolInfo().IsGenericType);
        }

        [Fact]
        public void IsGenericTypeWhenIB()
        {
            var iaTypeDefinition = typeof(IB<Test>).GetSymbolInfo();
            Assert.True(iaTypeDefinition.IsGenericType);
            Assert.True(iaTypeDefinition.IsInterface);
            Assert.Single(iaTypeDefinition.TypeArguments);
            var ta = iaTypeDefinition.TypeArguments.First();
            Assert.Equal(nameof(Test), ta.Name);
            Assert.Single(iaTypeDefinition.TypeParameters);
            var tp = iaTypeDefinition.TypeParameters.First();
            Assert.Equal(0, tp.Ordinal);
            Assert.Equal("T", tp.Name);
            Assert.Equal(RefKindInfo.In, tp.RefKind);
            Assert.False(tp.HasConstructorConstraint);
            Assert.False(tp.HasReferenceTypeConstraint);
            Assert.False(tp.HasValueTypeConstraint);
            Assert.Empty(tp.ConstraintTypes);
        }

        [Fact]
        public void IsGenericTypeWhenGenericClass()
        {
            var iaTypeDefinition = typeof(GenericClass<Test, B>).GetSymbolInfo();
            Assert.True(iaTypeDefinition.IsGenericType);
            Assert.True(iaTypeDefinition.IsClass);
            Assert.Equal(2, iaTypeDefinition.TypeArguments.Length);
            var ta = iaTypeDefinition.TypeArguments.First();
            Assert.Equal(nameof(Test), ta.Name);
            Assert.Equal(2, iaTypeDefinition.TypeParameters.Length);
            var tp = iaTypeDefinition.TypeParameters.First();
            Assert.Equal(0, tp.Ordinal);
            Assert.Equal("T", tp.Name);
            Assert.Equal(RefKindInfo.None, tp.RefKind);
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
            Assert.Equal(RefKindInfo.None, tp.RefKind);
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
            Assert.Null(typeof(object).GetSymbolInfo().BaseType);
            Assert.Equal(nameof(ValueType), typeof(int).GetSymbolInfo().BaseType.Name);
            Assert.Equal(nameof(Object), typeof(AbstractPublicClass).GetSymbolInfo().BaseType.Name);
            Assert.Equal(nameof(AbstractPublicClass), typeof(Test).GetSymbolInfo().BaseType.Name);
            Assert.Equal(nameof(ValueType), typeof(A).GetSymbolInfo().BaseType.Name);
        }

        [Fact]
        public void WhenInterfaces()
        {
            var interfaces = typeof(object).GetSymbolInfo().GetInterfaces();
            Assert.Empty(interfaces);
            interfaces = typeof(int).GetSymbolInfo().GetInterfaces();
            Assert.Contains(nameof(IComparable), interfaces.Select(i => i.Name));
            interfaces = typeof(Test).GetSymbolInfo().GetInterfaces();
            Assert.Empty(interfaces);
            interfaces = typeof(A).GetSymbolInfo().GetInterfaces();
            Assert.Empty(interfaces);
        }

        [Fact]
        public void WhenClassFields()
        {
            var fields = typeof(FieldTest).GetSymbolInfo().GetMembers()
                .Select(i => i as IFieldSymbolInfo)
                .Where(i => i != null)
                .ToDictionary(i => i.Name, i => i);
            Assert.Equal(6, fields.Count);
            var f = fields["A"];
            Assert.Equal(3, f.ConstantValue);
            Assert.True(f.IsConst);
            Assert.False(f.IsReadOnly);
            Assert.False(f.IsVolatile);
            Assert.False(f.IsFixedSizeBuffer);
            Assert.True(f.HasConstantValue);
            Assert.True(f.IsStatic);
            Assert.Equal(AccessibilityInfo.Public, f.Accessibility);

            f = fields["B"];
            Assert.False(f.HasConstantValue);
            Assert.False(f.IsConst);
            Assert.True(f.IsReadOnly);
            Assert.False(f.IsVolatile);
            Assert.False(f.IsFixedSizeBuffer);
            Assert.True(f.IsStatic);
            Assert.Equal(AccessibilityInfo.Internal, f.Accessibility);

            f = fields["C"];
            Assert.False(f.HasConstantValue);
            Assert.False(f.IsConst);
            Assert.False(f.IsReadOnly);
            Assert.True(f.IsVolatile);
            Assert.False(f.IsFixedSizeBuffer);
            Assert.False(f.IsStatic);
            Assert.Equal(AccessibilityInfo.Protected, f.Accessibility);

            f = fields["D"];
            Assert.False(f.HasConstantValue);
            Assert.False(f.IsConst);
            Assert.False(f.IsReadOnly);
            Assert.False(f.IsVolatile);
            Assert.False(f.IsFixedSizeBuffer);
            Assert.False(f.IsStatic);
            Assert.Equal(AccessibilityInfo.ProtectedOrInternal, f.Accessibility);

            f = fields["E"];
            Assert.False(f.HasConstantValue);
            Assert.False(f.IsConst);
            Assert.False(f.IsReadOnly);
            Assert.False(f.IsVolatile);
            Assert.False(f.IsFixedSizeBuffer);
            Assert.False(f.IsStatic);
            Assert.Equal(AccessibilityInfo.ProtectedAndInternal, f.Accessibility);

            f = fields["F"];
            Assert.False(f.HasConstantValue);
            Assert.False(f.IsConst);
            Assert.False(f.IsReadOnly);
            Assert.False(f.IsVolatile);
            Assert.False(f.IsFixedSizeBuffer);
            Assert.False(f.IsStatic);
            Assert.Equal(AccessibilityInfo.Private, f.Accessibility);
        }

        [Fact]
        public void WhenStructFields()
        {
            var fields = typeof(StructFieldTest).GetSymbolInfo().GetMembers()
                .Select(i => i as IFieldSymbolInfo)
                .Where(i => i != null)
                .ToDictionary(i => i.Name, i => i);
            Assert.Single(fields);
            var f = fields["name"];
            Assert.False(f.HasConstantValue);
            Assert.False(f.IsConst);
            Assert.False(f.IsReadOnly);
            Assert.False(f.IsVolatile);
            Assert.True(f.IsFixedSizeBuffer);
            Assert.False(f.IsStatic);
            Assert.Equal(AccessibilityInfo.Internal, f.Accessibility);
        }

        [Fact]
        public void WhenAttribute()
        {
            var attrs = typeof(AbstractPublicClass.PublicClass).GetSymbolInfo().GetAttributes();
            Assert.Single(attrs);
            var a = attrs.First();
            Assert.Equal(@"[Xunit.CollectionAttribute(""a"")]", a.FullName);
            Assert.Equal(@"Xunit.CollectionAttribute", a.AttributeType.FullName);
            Assert.Single(a.ConstructorArguments);
            var ca = a.ConstructorArguments.First();
            Assert.Equal("a", ca.Value);
        }
    }
}