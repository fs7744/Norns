using System;
using System.Text;

namespace Norns.Destiny.Notations
{
    public class ActionNotation : INotation
    {
        private readonly Action<StringBuilder> action;

        public ActionNotation(Action<StringBuilder> action)
        {
            this.action = action;
        }

        public void Record(StringBuilder sb)
        {
            action(sb);
        }
    }
}