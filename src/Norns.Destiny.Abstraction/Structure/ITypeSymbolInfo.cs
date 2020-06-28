﻿using System.Collections.Immutable;

namespace Norns.Destiny.Abstraction.Structure
{
    public interface ITypeSymbolInfo : ISymbolInfo
    {
        string Namespace { get; }
        bool IsAbstract { get; }
        bool IsSealed { get; }
        bool IsValueType { get; }
        bool IsGenericType { get; }
        ImmutableArray<ITypeSymbolInfo> TypeArguments { get; }
        ImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; }
        bool IsAnonymousType { get; }
        bool IsClass { get; }
        bool IsInterface { get; }
        ITypeSymbolInfo BaseType { get; }
        bool IsStatic { get; }
        AccessibilityInfo Accessibility { get; }
        string GenericDefinitionName { get; }

        ImmutableArray<ITypeSymbolInfo> GetInterfaces();

        ImmutableArray<ISymbolInfo> GetMembers();

        ImmutableArray<IAttributeSymbolInfo> GetAttributes();
    }
}