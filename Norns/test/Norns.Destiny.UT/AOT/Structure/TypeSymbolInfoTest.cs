using Norns.Destiny.Abstraction.Structure;
using System;
using System.Linq;
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
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code, false);
            var iaTypeDefinition = types["Norns.Destiny.UT.AOT.Generated.IA<T>"];
            Assert.True(iaTypeDefinition.IsGenericType);
            Assert.Single(iaTypeDefinition.TypeArguments);
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
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code);
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
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code);
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
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.Empty(types["ClassT"].GetInterfaces());
            Assert.Single(types["C"].GetInterfaces());
            Assert.Equal("IB", types["C"].GetInterfaces().First().Name);
            Assert.Empty(types["A"].GetInterfaces());
            Assert.Empty(types["IB"].GetInterfaces());
            Assert.True(types["IB"].IsInterface);
        }

        [Fact]
        public void WhenFields()
        {
            var code = @"public class ClassT { public int x, y; }";
            var types = AotTest.SimpleGenerateTypeSymbolInfos(code);
            Assert.Empty(types["ClassT"].GetInterfaces());
        }
    }
}