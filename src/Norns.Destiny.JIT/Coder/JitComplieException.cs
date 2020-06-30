using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Norns.Destiny.JIT.Coder
{
    public class JitComplieException : Exception
    {
        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public JitComplieException(ImmutableArray<Diagnostic> diagnostics) : base(diagnostics.Select(i => i.ToString()).DefaultIfEmpty().Aggregate((i, j) => i + j))
        {
            Diagnostics = diagnostics;
        }
    }
}