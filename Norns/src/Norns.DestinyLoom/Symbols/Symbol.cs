using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Norns.DestinyLoom.Symbols
{
    public static class Symbol
    {
        public static readonly StringSymbol KeyClass = "class".ToSymbol();
        public static readonly StringSymbol KeyNamespace = "namespace".ToSymbol();
        public static readonly StringSymbol KeyBlank = " ".ToSymbol();
        public static readonly StringSymbol KeyComma = ",".ToSymbol();
        public static readonly StringSymbol KeyColon = ":".ToSymbol();
        public static readonly StringSymbol KeyOpenBrace = "{".ToSymbol();
        public static readonly StringSymbol KeyCloseBrace = "}".ToSymbol();
        public static readonly StringSymbol KeyOpenAngleBracket = "<".ToSymbol();
        public static readonly StringSymbol KeyCloseAngleBracket = ">".ToSymbol();
        public static readonly StringSymbol KeyOpenParen = "(".ToSymbol();
        public static readonly StringSymbol KeyCloseParen = ")".ToSymbol();
        public static readonly StringSymbol KeyOpenBracket = "[".ToSymbol();
        public static readonly StringSymbol KeyCloseBracket = "]".ToSymbol();
        public static readonly StringSymbol KeySemicolon = ";".ToSymbol();
        public static readonly StringSymbol KeyThis = "this".ToSymbol();
        public static readonly StringSymbol KeyAsync = "async".ToSymbol();
        public static readonly StringSymbol KeyOverride = "override".ToSymbol();
        public static readonly StringSymbol KeyUsing = "using".ToSymbol();

        public static string ToDisplayString(this Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    return "private";

                case Accessibility.ProtectedAndInternal:
                    return "private protected";

                case Accessibility.Protected:
                    return "protected";

                case Accessibility.Internal:
                    return "internal";

                case Accessibility.ProtectedOrInternal:
                    return "protected internal";

                case Accessibility.Public:
                    return "public";

                default:
                    return string.Empty;
            }
        }

        public static IGenerateSymbol Create(Func<IGenerateSymbol> func)
        {
            return new SymbolLazyCreater(func);
        }

        public static UsingSymbol CreateUsing(string @namespace)
        {
            return new UsingSymbol() { Name = @namespace };
        }

        public static NamespaceSymbol CreateNamespace(string @namespace)
        {
            return new NamespaceSymbol() { Name = @namespace };
        }

        public static ClassSymbol CreateClass(string @class, string accessibility)
        {
            return new ClassSymbol() { Name = @class, Accessibility = accessibility };
        }

        public static PropertySymbol CreateProperty(string accessibility, string type, string name)
        {
            return new PropertySymbol()
            {
                Accessibility = accessibility,
                Type = type,
                Name = name
            };
        }

        public static MethodSymbol CreateMethod(string accessibility, string returnType, string name)
        {
            return new MethodSymbol()
            {
                Accessibility = accessibility,
                ReturnType = returnType,
                Name = name
            };
        }

        public static ParameterSymbol CreateParameter(string type, string name)
        {
            return new ParameterSymbol() { Type = type, Name = name };
        }

        public static FieldSymbol CreateField(string accessibility, string type, string name)
        {
            return new FieldSymbol()
            {
                Accessibility = accessibility,
                Type = type,
                Name = name
            };
        }

        public static IGenerateSymbol WithComma(this StringSymbol symbol)
        {
            return WithComma(symbol, symbol.CreateCanMergeFunc());
        }

        public static IGenerateSymbol WithComma(this IGenerateSymbol symbol, Func<bool> canMerge)
        {
            return Merge(null, canMerge, symbol, KeyComma);
        }

        public static IGenerateSymbol WithFullBlank(this StringSymbol symbol)
        {
            return Merge(null, symbol.CreateCanMergeFunc(), KeyBlank, symbol, KeyBlank);
        }

        public static IGenerateSymbol WithBlank(this StringSymbol symbol)
        {
            return WithBlank(symbol, symbol.CreateCanMergeFunc());
        }

        public static IGenerateSymbol WithBlank(this IGenerateSymbol symbol, Func<bool> canMerge)
        {
            return Merge(canMerge, symbol, KeyBlank);
        }

        public static Func<bool> CreateCanMergeFunc(this StringSymbol symbol)
        {
            return () => !string.IsNullOrEmpty(symbol.Value);
        }

        public static IGenerateSymbol Merge(params IGenerateSymbol[] symbols)
        {
            return Merge(null, () => true, symbols);
        }

        public static IGenerateSymbol Merge(Func<bool> canMerge, params IGenerateSymbol[] symbols)
        {
            return Merge(null, canMerge, symbols);
        }

        public static IGenerateSymbol Merge(IGenerateSymbol separator, Func<bool> canMerge, params IGenerateSymbol[] symbols)
        {
            return new SymbolMerger(canMerge, symbols) { Separator = separator };
        }

        public static StringSymbol ToSymbol(this string value)
        {
            return new StringSymbol(value);
        }

        public static GenerateSymbolLinkedList ToSymbol(this IEnumerable<string> values)
        {
            return values.Select(i => i.ToSymbol()).ToSymbol();
        }

        public static GenerateSymbolLinkedList ToSymbol(this IEnumerable<IGenerateSymbol> values)
        {
            return new GenerateSymbolLinkedList().AddSymbols(values);
        }

        public static GenerateSymbolLinkedList AddSymbols(this GenerateSymbolLinkedList generateSymbols, IEnumerable<IGenerateSymbol> values)
        {
            return values.Aggregate(generateSymbols, (n, i) =>
                {
                    n.AddLast(i);
                    return n;
                });
        }

        public static IGenerateSymbol GenerateSymbolBeforeMethod(this IInterceptorGenerator[] interceptors, ProxyMethodGeneratorContext context)
        {
            return interceptors.Select(i => i.BeforeMethodGenerateSymbol(context)).ToSymbol();
        }

        public static IGenerateSymbol GenerateSymbolAfterMethod(this IInterceptorGenerator[] interceptors, ProxyMethodGeneratorContext context)
        {
            return interceptors.Select(i => i.AfterMethodGenerateSymbol(context)).ToSymbol();
        }
    }
}