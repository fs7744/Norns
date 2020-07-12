using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Norns.Verthandi.Loom
{
    public class ComplieException : Exception
    {
        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public ComplieException(ImmutableArray<Diagnostic> diagnostics) : base(diagnostics.Select(i => i.ToString()).DefaultIfEmpty().Aggregate((i, j) => i + j))
        {
            Diagnostics = diagnostics;
        }
    }
}