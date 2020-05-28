using System;

namespace TestIOC
{
    public class ReplaceAttribute : Attribute
    {
        public ReplaceAttribute(Type realType)
        {
            RealType = realType;
        }

        public Type RealType { get; }
    }
}