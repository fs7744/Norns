using Norns.Destiny.RuntimeSymbol;
using Norns.Destiny.Structure;
using System;
using System.Linq;
using Xunit;

namespace Norns.Destiny.UT.RuntimeSymbol
{
    public static class ParameterTest
    {
        public static void A(params int[] vs)
        {
            vs.ToString();
        }

        public static void B(this int _)
        {
            _.ToString();
        }
    }

    public class ParameterSymbolInfoTest
    {
        [Fact]
        public void WhenIsParams()
        {
            var ps = typeof(ParameterTest).GetSymbolInfo().Members
                .Select(i => i as IMethodSymbolInfo)
                .Where(i => i != null)
                .First(i => i.Name == "A")
                .Parameters;
            Assert.Single(ps);
            var p = ps.First();
            Assert.Equal("vs", p.Name);
            Assert.True(p.IsParams);
            Assert.False(p.IsOptional);
            Assert.False(p.HasExplicitDefaultValue);
            Assert.Equal(DBNull.Value, p.ExplicitDefaultValue);
            Assert.Equal(RefKindInfo.None, p.RefKind);
            Assert.Equal("Int32[]", p.Type.Name);
            Assert.Equal(0, p.Ordinal);
        }

        [Fact]
        public void WhenIsThis()
        {
            var m = typeof(ParameterTest).GetSymbolInfo().Members
                   .Select(i => i as IMethodSymbolInfo)
                   .Where(i => i != null)
                   .First(i => i.Name == "B");
            Assert.True(m.IsExtensionMethod);
            var ps = m.Parameters;
            Assert.Single(ps);
            var p = ps.First();
            Assert.Equal("_", p.Name);
            Assert.False(p.IsParams);
            Assert.False(p.IsOptional);
            Assert.False(p.HasExplicitDefaultValue);
            Assert.Equal(DBNull.Value, p.ExplicitDefaultValue);
            Assert.Equal(RefKindInfo.None, p.RefKind);
            Assert.Equal("Int32", p.Type.Name);
            Assert.Equal(0, p.Ordinal);
        }
    }
}