using Norns.Destiny.RuntimeSymbol;
using Norns.Destiny.Structure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Norns.Destiny.UT.RuntimeSymbol
{
    public static class Sta
    {
        public static int A(this int d) => d;
    }

    public delegate void TestA(int a);

    public class MethodSymbolInfoTest
    {
        public abstract class A
        {
            public event TestA EventTestA;

            private void PrivateM()
            {
                EventTestA(3);
            }

            internal int InternalM()
            {
                return default;
            }

            protected virtual (int v, long s) ProtectedM()
            {
                return default;
            }

            protected internal abstract string PIS();

            protected private string PPS()
            {
                return default;
            }

            public T GetT<T>() where T : class
            {
                return default;
            }

            public Task GetTask()
            {
                return default;
            }

            public override string ToString() => nameof(A);
        }

        public class B : A
        {
            protected sealed override (int v, long s) ProtectedM()
            {
                return base.ProtectedM();
            }

            protected internal override string PIS()
            {
                return default;
            }

            public new Task GetTask()
            {
                return default;
            }
        }

        [Fact]
        public void WhenMethods()
        {
            var ms = typeof(A).GetSymbolInfo().Members
                 .Select(i => i as IMethodSymbolInfo)
                 .Where(i => i != null)
                 .ToDictionary(i => $"{i.ContainingType.FullName}.{i.Name}", i => i);
            var m = ms["Norns.Destiny.UT.RuntimeSymbol.MethodSymbolInfoTest.A.PrivateM"];
            Assert.Equal(AccessibilityInfo.Private, m.Accessibility);
            Assert.Empty(m.Parameters);
            Assert.Empty(m.TypeParameters);
            Assert.False(m.IsGenericMethod);
            Assert.False(m.IsExtensionMethod);
            Assert.False(m.IsStatic);
            Assert.False(m.IsSealed);
            Assert.False(m.IsAbstract);
            Assert.False(m.IsOverride);
            Assert.False(m.IsVirtual);
            Assert.Equal("void", m.ReturnType.FullName);

            m = ms["Norns.Destiny.UT.RuntimeSymbol.MethodSymbolInfoTest.A.InternalM"];
            Assert.Equal(AccessibilityInfo.Internal, m.Accessibility);
            Assert.Empty(m.Parameters);
            Assert.Empty(m.TypeParameters);
            Assert.False(m.IsGenericMethod);
            Assert.False(m.IsExtensionMethod);
            Assert.False(m.IsStatic);
            Assert.False(m.IsSealed);
            Assert.False(m.IsAbstract);
            Assert.False(m.IsOverride);
            Assert.False(m.IsVirtual);
            Assert.Equal("int", m.ReturnType.FullName);

            m = ms["Norns.Destiny.UT.RuntimeSymbol.MethodSymbolInfoTest.A.ProtectedM"];
            Assert.Equal(AccessibilityInfo.Protected, m.Accessibility);
            Assert.Empty(m.Parameters);
            Assert.Empty(m.TypeParameters);
            Assert.False(m.IsGenericMethod);
            Assert.False(m.IsExtensionMethod);
            Assert.False(m.IsStatic);
            Assert.False(m.IsSealed);
            Assert.False(m.IsAbstract);
            Assert.False(m.IsOverride);
            Assert.True(m.IsVirtual);
            Assert.Equal("ValueTuple", m.ReturnType.Name);

            m = ms["Norns.Destiny.UT.RuntimeSymbol.MethodSymbolInfoTest.A.PIS"];
            Assert.Equal(AccessibilityInfo.ProtectedOrInternal, m.Accessibility);
            Assert.Empty(m.Parameters);
            Assert.Empty(m.TypeParameters);
            Assert.False(m.IsGenericMethod);
            Assert.False(m.IsExtensionMethod);
            Assert.False(m.IsStatic);
            Assert.False(m.IsSealed);
            Assert.True(m.IsAbstract);
            Assert.False(m.IsOverride);
            Assert.False(m.IsVirtual);
            Assert.Equal("String", m.ReturnType.Name);

            m = ms["Norns.Destiny.UT.RuntimeSymbol.MethodSymbolInfoTest.A.PPS"];
            Assert.Equal(AccessibilityInfo.ProtectedAndInternal, m.Accessibility);
            Assert.Empty(m.Parameters);
            Assert.Empty(m.TypeParameters);
            Assert.False(m.IsGenericMethod);
            Assert.False(m.IsExtensionMethod);
            Assert.False(m.IsStatic);
            Assert.False(m.IsSealed);
            Assert.False(m.IsAbstract);
            Assert.False(m.IsOverride);
            Assert.False(m.IsVirtual);
            Assert.Equal("String", m.ReturnType.Name);

            m = ms["Norns.Destiny.UT.RuntimeSymbol.MethodSymbolInfoTest.A.ToString"];
            Assert.Equal(AccessibilityInfo.Public, m.Accessibility);
            Assert.Empty(m.Parameters);
            Assert.Empty(m.TypeParameters);
            Assert.False(m.IsGenericMethod);
            Assert.False(m.IsExtensionMethod);
            Assert.False(m.IsStatic);
            Assert.False(m.IsSealed);
            Assert.False(m.IsAbstract);
            Assert.True(m.IsOverride);
            Assert.True(m.IsVirtual);
            Assert.Equal("String", m.ReturnType.Name);

            m = ms["Norns.Destiny.UT.RuntimeSymbol.MethodSymbolInfoTest.A.GetTask"];
            Assert.Equal(AccessibilityInfo.Public, m.Accessibility);
            Assert.Empty(m.Parameters);
            Assert.Empty(m.TypeParameters);
            Assert.False(m.IsGenericMethod);
            Assert.False(m.IsExtensionMethod);
            Assert.False(m.IsStatic);
            Assert.False(m.IsSealed);
            Assert.False(m.IsAbstract);
            Assert.False(m.IsOverride);
            Assert.False(m.IsVirtual);
            Assert.Equal("Task", m.ReturnType.Name);

            m = ms["Norns.Destiny.UT.RuntimeSymbol.MethodSymbolInfoTest.A.GetT"];
            Assert.Equal(AccessibilityInfo.Public, m.Accessibility);
            Assert.Empty(m.Parameters);
            Assert.True(m.IsGenericMethod);
            Assert.False(m.IsExtensionMethod);
            Assert.False(m.IsStatic);
            Assert.False(m.IsSealed);
            Assert.False(m.IsAbstract);
            Assert.False(m.IsOverride);
            Assert.False(m.IsVirtual);
            Assert.Equal("T", m.ReturnType.Name);
            Assert.Single(m.TypeParameters);
            Assert.True(m.TypeParameters.First().HasReferenceTypeConstraint);

            ms = typeof(B).GetSymbolInfo().Members
                 .Select(i => i as IMethodSymbolInfo)
                 .Where(i => i != null)
                 .ToDictionary(i => $"{i.ContainingType.FullName}.{i.Name}", i => i);
            m = ms["Norns.Destiny.UT.RuntimeSymbol.MethodSymbolInfoTest.B.ProtectedM"];
            Assert.Equal(AccessibilityInfo.Protected, m.Accessibility);
            Assert.Empty(m.Parameters);
            Assert.Empty(m.TypeParameters);
            Assert.False(m.IsGenericMethod);
            Assert.False(m.IsExtensionMethod);
            Assert.False(m.IsStatic);
            Assert.True(m.IsSealed);
            Assert.False(m.IsAbstract);
            Assert.True(m.IsOverride);
            Assert.True(m.IsVirtual);
            Assert.Equal("ValueTuple", m.ReturnType.Name);

            m = ms["Norns.Destiny.UT.RuntimeSymbol.MethodSymbolInfoTest.B.PIS"];
            Assert.Equal(AccessibilityInfo.ProtectedOrInternal, m.Accessibility);
            Assert.Empty(m.Parameters);
            Assert.Empty(m.TypeParameters);
            Assert.False(m.IsGenericMethod);
            Assert.False(m.IsExtensionMethod);
            Assert.False(m.IsStatic);
            Assert.False(m.IsSealed);
            Assert.False(m.IsAbstract);
            Assert.True(m.IsOverride);
            Assert.True(m.IsVirtual);
            Assert.Equal("String", m.ReturnType.Name);

            m = ms["Norns.Destiny.UT.RuntimeSymbol.MethodSymbolInfoTest.B.GetTask"];
            Assert.Equal(AccessibilityInfo.Public, m.Accessibility);
            Assert.Empty(m.Parameters);
            Assert.Empty(m.TypeParameters);
            Assert.False(m.IsGenericMethod);
            Assert.False(m.IsExtensionMethod);
            Assert.False(m.IsStatic);
            Assert.False(m.IsSealed);
            Assert.False(m.IsAbstract);
            Assert.False(m.IsOverride);
            Assert.False(m.IsVirtual);
            Assert.Equal("Task", m.ReturnType.Name);

            ms = typeof(Sta).GetSymbolInfo().Members
                 .Select(i => i as IMethodSymbolInfo)
                 .Where(i => i != null)
                 .ToDictionary(i => $"{i.ContainingType.FullName}.{i.Name}", i => i);
            m = ms["Norns.Destiny.UT.RuntimeSymbol.Sta.A"];
            Assert.Equal(AccessibilityInfo.Public, m.Accessibility);
            Assert.Empty(m.TypeParameters);
            Assert.False(m.IsGenericMethod);
            Assert.True(m.IsExtensionMethod);
            Assert.True(m.IsStatic);
            Assert.False(m.IsSealed);
            Assert.False(m.IsAbstract);
            Assert.False(m.IsOverride);
            Assert.False(m.IsVirtual);
            Assert.Equal("Int32", m.ReturnType.Name);
            Assert.Single(m.Parameters);
            Assert.Equal("d", m.Parameters.First().Name);
        }
    }
}