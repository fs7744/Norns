using Norns.Destiny.Notations;
using System.Text;
using Xunit;

namespace Norns.Destiny.UT.Notations
{
    public class NotationTest
    {
        [Fact]
        public void CombineWhenListEmpty()
        {
            var result = new INotation[0].Combine();
            Assert.IsType<ActionNotation>(result);
            var sb = new StringBuilder();
            result.Record(sb);
            Assert.Empty(sb.ToString());
        }

        [Fact]
        public void CombineWhenListNotEmpty()
        {
            var result = new INotation[] { "a".ToNotation(), "b".ToNotation() }.Combine();
            Assert.NotSame(ConstNotations.Nothing, result);
            var sb = new StringBuilder();
            result.Record(sb);
            Assert.Equal("ab", sb.ToString());
        }
    }
}