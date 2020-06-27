using System.Collections.Generic;
using System.Text;

namespace Norns.Destiny.Notations
{
    public abstract class MembersNotation : INotation
    {
        public abstract IEnumerable<INotation> GetMembers();

        public void Record(StringBuilder sb)
        {
            GetMembers().Combine().Record(sb);
        }
    }
}