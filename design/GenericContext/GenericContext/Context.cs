namespace GenericContext
{
    public abstract class Context
    {
    }

    public class VoidContext: Context
    {
    }

    public class Context<TParam, TResult> : Context
    {
        public TParam Parameters { get; set; }

        public TResult Result { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}