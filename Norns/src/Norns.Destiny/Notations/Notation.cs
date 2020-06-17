namespace Norns.Destiny.Notations
{
    public static class Notation
    {
        public static INotation Combine(INotation x, INotation y)
        {
            return new ActionNotation(sb =>
            {
                x.Record(sb);
                y.Record(sb);
            });
        }
    }
}