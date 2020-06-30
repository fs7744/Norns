using System.Collections.Generic;

namespace Norns.Destiny.Notations
{
    public static class ConstNotations
    {
        /// <summary>
        /// .
        /// </summary>
        public static readonly INotation Dot = ".".ToNotation();

        /// <summary>
        /// ' '
        /// </summary>
        public static readonly INotation Blank = " ".ToNotation();

        /// <summary>
        /// ,
        /// </summary>
        public static readonly INotation Comma = ",".ToNotation();

        /// <summary>
        /// :
        /// </summary>
        public static readonly INotation Colon = ":".ToNotation();

        /// <summary>
        /// {
        /// </summary>
        public static readonly INotation OpenBrace = "{".ToNotation();

        /// <summary>
        /// }
        /// </summary>
        public static readonly INotation CloseBrace = "}".ToNotation();

        /// <summary>
        /// '<'
        /// </summary>
        public static readonly INotation OpenAngleBracket = "<".ToNotation();

        /// <summary>
        /// '>'
        /// </summary>
        public static readonly INotation CloseAngleBracket = ">".ToNotation();

        /// <summary>
        /// (
        /// </summary>
        public static readonly INotation OpenParen = "(".ToNotation();

        /// <summary>
        /// )
        /// </summary>
        public static readonly INotation CloseParen = ")".ToNotation();

        /// <summary>
        /// [
        /// </summary>
        public static readonly INotation OpenBracket = "[".ToNotation();

        /// <summary>
        /// ]
        /// </summary>
        public static readonly INotation CloseBracket = "]".ToNotation();

        /// <summary>
        /// ;
        /// </summary>
        public static readonly INotation Semicolon = ";".ToNotation();

        /// <summary>
        /// this
        /// </summary>
        public static readonly INotation This = "this".ToNotation();

        /// <summary>
        /// async
        /// </summary>
        public static readonly INotation Async = "async".ToNotation();

        /// <summary>
        /// override
        /// </summary>
        public static readonly INotation Override = "override".ToNotation();

        /// <summary>
        /// using
        /// </summary>
        public static readonly INotation Using = "using".ToNotation();

        public static readonly INotation Nothing = string.Empty.ToNotation();

        public static readonly IEnumerable<INotation> Nothings = new INotation[] { Nothing };

        public static readonly INotation Where = "where".ToNotation();

        public static readonly INotation Class = "class".ToNotation();
       
        public static readonly INotation Struct = "struct".ToNotation();
        
        public static readonly INotation ConstructorConstraint = "new()".ToNotation();

        public static readonly INotation Base = "base".ToNotation();
    }
}