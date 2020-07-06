using Norns.Destiny.Structure;
using System;
using System.Linq;
using Xunit;

namespace Norns.Destiny.UT.Skuld.Structure
{
    public class TypeSymbolInfoTest
    {
        [Fact]
        public void WhenAttribute()
        {
            var code = @"
using Xunit;
[Collection(""a"")]
        public class PublicClass
        { }
    }";
            var attrs = SkuldTest.SimpleGenerateTypeSymbolInfos(code).First().Value.Attributes;
            Assert.Single(attrs);
            var a = attrs.First();
            Assert.Equal(@"Xunit.CollectionAttribute(""a"")", a.FullName);
            Assert.Equal(@"Xunit.CollectionAttribute", a.AttributeType.FullName);
            Assert.Single(a.ConstructorArguments);
            var ca = a.ConstructorArguments.First();
            Assert.Equal("a", ca.Value);
        }

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
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
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
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.Equal(AccessibilityInfo.Internal, types["InternalClass"].Accessibility);
            Assert.Equal(AccessibilityInfo.Public, types["PublicClass"].Accessibility);
        }

        #endregion Accessibility

        [Fact]
        public void WhenIsStatic()
        {
            var code = "public static class StaticClass {} class NonStaticClass{}";
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.True(types["StaticClass"].IsStatic);
            Assert.False(types["NonStaticClass"].IsStatic);
        }

        [Fact]
        public void WhenIsAnonymousType()
        {
            var code = "public static class StaticClass { void A() { var a = new {}; var b = new { A = 9}; } } ";
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.False(types["StaticClass"].IsAnonymousType);
        }

        [Fact]
        public void WhenNameAndNamespace()
        {
            var code = "public class ADD { } ";
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.Equal("Norns.Destiny.UT.AOT.Generated", types["ADD"].Namespace);
        }

        [Fact]
        public void WhenIsSealed()
        {
            var code = "public sealed class SealedClass { } public class NonSealedClass { }";
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.True(types["SealedClass"].IsSealed);
            Assert.False(types["NonSealedClass"].IsSealed);
        }

        [Fact]
        public void WhenIsAbstract()
        {
            var code = "public abstract class AbstractClass { } public class NonAbstractClass { }";
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.True(types["AbstractClass"].IsAbstract);
            Assert.False(types["NonAbstractClass"].IsAbstract);
        }

        [Fact]
        public void WhenIsValueType()
        {
            var code = "public struct MyStruct { } public class ClassT { }";
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.True(types["MyStruct"].IsValueType);
            Assert.False(types["ClassT"].IsValueType);
        }

        #region IsGenericType

        [Fact]
        public void IsGenericTypeWhenIA()
        {
            var code = @"public class PublicClass
    { }     public class IA<out T> where T : PublicClass, new()
    {
        public T A() => new IA<PublicClass>();
    }
            ";
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code, false);
            var iaTypeDefinition = types["Norns.Destiny.UT.AOT.Generated.IA<T>"];
            Assert.True(iaTypeDefinition.IsGenericType);
            Assert.Single(iaTypeDefinition.TypeArguments);
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
            Assert.Equal("PublicClass", tpc.Name);
        }

        [Fact]
        public void WhenIsGenericType()
        {
            var code = @"public class ClassT { }

public class GenericClass<T> where T : ClassT, new()
{
    public T A() => default;
}";
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.True(types["GenericClass"].IsGenericType);
            Assert.True(types["GenericClass"].IsClass);
            Assert.False(types["ClassT"].IsGenericType);
        }

        #endregion IsGenericType

        [Fact]
        public void WhenBaseType()
        {
            var code = @"public class ClassT { } public struct A {} interface IB {}
public class C : ClassT {}";
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.Equal(nameof(Object), types["ClassT"].BaseType.Name);
            Assert.Equal("ClassT", types["C"].BaseType.Name);
            Assert.Equal("ValueType", types["A"].BaseType.Name);
            Assert.Null(types["IB"].BaseType);
        }

        [Fact]
        public void WhenInterfaces()
        {
            var code = @"public class ClassT { } public struct A {} interface IB {}
public class C : IB {}";
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.Empty(types["ClassT"].Interfaces);
            Assert.Single(types["C"].Interfaces);
            Assert.Equal("IB", types["C"].Interfaces.First().Name);
            Assert.Empty(types["A"].Interfaces);
            Assert.Empty(types["IB"].Interfaces);
            Assert.True(types["IB"].IsInterface);
        }

        [Fact]
        public void WhenFields()
        {
            var code = @"public class ClassT { public int x, y; }";
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.Empty(types["ClassT"].Interfaces);
        }

        [Fact]
        public void WhenClassFields()
        {
            var code = @"public class FieldTest
        {
            public const int A = 3;
            internal static readonly string B = ""3"";
            protected volatile string C;
        protected internal long D;
        private protected long E;
        private long F;
    }";
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
            var fields = types["FieldTest"].Members
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
            var code = @"public unsafe struct StructFieldTest
        {
            internal fixed char name[30];
        }";
            var types = SkuldTest.SimpleGenerateTypeSymbolInfos(code);
            var fields = types["StructFieldTest"].Members
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
    }
}