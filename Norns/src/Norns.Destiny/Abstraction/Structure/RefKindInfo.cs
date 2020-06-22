namespace Norns.Destiny.Abstraction.Structure
{
    public enum RefKindInfo : byte
    {
        None = 0,

        //     Indicates a "ref" parameter or return type.
        Ref = 1,

        //     Indicates an "out" parameter.
        Out = 2,

        //     Indicates an "in" parameter.
        In = 3,
    }
}