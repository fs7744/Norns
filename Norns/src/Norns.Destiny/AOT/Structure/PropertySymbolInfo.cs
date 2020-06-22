﻿using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Structure;
using System.Collections.Immutable;

namespace Norns.Destiny.AOT.Structure
{
    public class PropertySymbolInfo : IPropertySymbolInfo
    {
        public PropertySymbolInfo(IPropertySymbol p)
        {
            RealProperty = p;
            Type = new TypeSymbolInfo(p.Type);
            Accessibility = p.DeclaredAccessibility.ConvertToStructure();
            GetMethod = CanRead ? new MethodSymbolInfo(p.GetMethod) : null;
            SetMethod = CanWrite ? new MethodSymbolInfo(p.SetMethod) : null;
        }

        private IPropertySymbol RealProperty { get; }
        public object Origin => RealProperty;
        public string Name => RealProperty.Name;
        public bool IsIndexer => RealProperty.IsIndexer;
        public bool CanWrite => !RealProperty.IsReadOnly;
        public bool CanRead => !RealProperty.IsWriteOnly;
        public ITypeSymbolInfo Type { get; }
        public ImmutableArray<IParameterSymbolInfo> Parameters { get; }
        public AccessibilityInfo Accessibility { get; }
        public bool IsStatic => RealProperty.IsStatic;
        public bool IsExtern => RealProperty.IsExtern;
        public bool IsSealed => RealProperty.IsSealed;
        public bool IsAbstract => RealProperty.IsAbstract;
        public bool IsOverride => RealProperty.IsVirtual && RealProperty.IsOverride;
        public bool IsVirtual => RealProperty.IsVirtual;
        public bool IsNew => !RealProperty.IsVirtual && RealProperty.IsOverride;
        public IMethodSymbolInfo GetMethod { get; }
        public IMethodSymbolInfo SetMethod { get; }
    }
}