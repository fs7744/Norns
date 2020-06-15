using Microsoft.CodeAnalysis;
using Norns.DestinyLoom.Symbols;
using System.Collections.Generic;

namespace Norns.DestinyLoom
{
    public class ProxyGeneratorContext
    {
        public ProxyGeneratorContext(INamedTypeSymbol typeSymbol, SourceGeneratorContext context, string @namespace)
        {
            Type = typeSymbol;
            SourceGeneratorContext = context;
            Namespace = @namespace;
            ProxyFieldName = $"proxy{GuidHelper.NewGuidName()}";
        }

        public INamedTypeSymbol Type { get; }
        public SourceGeneratorContext SourceGeneratorContext { get; }
        public string Namespace { get; }
        public string ProxyFieldName { get; }
        public Dictionary<string, FieldSymbol> DIFields { get; } = new Dictionary<string, FieldSymbol>();
        public Dictionary<string, UsingSymbol> Usings { get; } = new Dictionary<string, UsingSymbol>();

        public string GetFromDI(string type)
        {
            if (DIFields.TryGetValue(type, out var f))
            {
                return f.Name;
            }
            else
            {
                var field = Symbol.CreateField("private", type, $"f{GuidHelper.NewGuidName()}");
                DIFields.Add(type, field);
                return field.Name;
            }
        }

        public void AddUsing(string @namespace)
        {
            if (!Usings.TryGetValue(@namespace, out var f))
            {
                Usings.Add(@namespace, Symbol.CreateUsing(@namespace));
            }
        }
    }
}