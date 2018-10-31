using System;

namespace Norns.AOP.Extensions
{
    public static class Arguments
    {
        public static void NotNullOrEmpty(string name, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}