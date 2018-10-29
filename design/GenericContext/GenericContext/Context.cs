using System.Runtime.CompilerServices;

namespace GenericContext
{
    public abstract class Context
    {
        public abstract ITuple Params { get; }

        public abstract object ObjectResult { get; }
    }

    public class Context<TParam, TResult> : Context where TParam : ITuple
    {
        public TParam Parameters { get; set; }

        public override ITuple Params => Parameters;

        public TResult Result { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public override object ObjectResult => Result;
    }
}