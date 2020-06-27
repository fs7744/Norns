using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace Norns.Destiny.JIT.Coder
{
    [Serializable]
    public class JitComplieException : Exception
    {
        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public JitComplieException()
        {
        }

        public JitComplieException(ImmutableArray<Diagnostic> diagnostics)
        {
            Diagnostics = diagnostics;
        }

        public JitComplieException(string message) : base(message)
        {
        }

        public JitComplieException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected JitComplieException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}