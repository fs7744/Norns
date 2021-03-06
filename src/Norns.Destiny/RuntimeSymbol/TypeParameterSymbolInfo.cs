﻿using Norns.Destiny.Immutable;
using Norns.Destiny.Structure;
using System;
using System.Linq;
using System.Reflection;

namespace Norns.Destiny.RuntimeSymbol
{
    public class TypeParameterSymbolInfo : TypeSymbolInfo, ITypeParameterSymbolInfo
    {
        public TypeParameterSymbolInfo(Type type) : base(type)
        {
            Ordinal = type.GenericParameterPosition;
            HasReferenceTypeConstraint = (type.GenericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint;
            HasValueTypeConstraint = (type.GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint;
            HasConstructorConstraint = (type.GenericParameterAttributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint;
            RefKind = type.GenericParameterAttributes.ConvertToStructure();
            ConstraintTypes = EnumerableExtensions.CreateLazyImmutableArray(() => RealType.GetGenericParameterConstraints().Where(i => i != typeof(ValueType)).Select(i => i.GetSymbolInfo()));
        }

        public int Ordinal { get; }
        public RefKindInfo RefKind { get; }
        public bool HasReferenceTypeConstraint { get; }
        public bool HasValueTypeConstraint { get; }
        public bool HasUnmanagedTypeConstraint { get; }
        public bool HasNotNullConstraint { get; }
        public bool HasConstructorConstraint { get; }
        public IImmutableArray<ITypeSymbolInfo> ConstraintTypes { get; }
    }
}