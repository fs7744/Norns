using Norns.Destiny.RuntimeSymbol;
using Norns.Destiny.Structure;
using System.Linq;
using Xunit;

namespace Norns.Destiny.UT.JIT.Structure
{
    public class PropertySymbolInfoTest
    {
        public class PropertyTest
        {
            public object this[int index]
            {
                private get { return null; }
                set { value?.ToString(); }
            }
        }

        [Fact]
        public void WhenIndexer()
        {
            var ps = typeof(PropertyTest).GetSymbolInfo().GetMembers()
                .Select(i => i as IPropertySymbolInfo)
                .Where(i => i != null)
                .ToDictionary(i => i.Name, i => i);
            var p = ps["Item"];
            Assert.True(p.IsIndexer);
            Assert.Equal(AccessibilityInfo.Public, p.Accessibility);
        }
    }
}