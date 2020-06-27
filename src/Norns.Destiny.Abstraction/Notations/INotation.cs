using System.Text;

namespace Norns.Destiny.Notations
{
    public interface INotation
    {
        void Record(StringBuilder sb);
    }
}