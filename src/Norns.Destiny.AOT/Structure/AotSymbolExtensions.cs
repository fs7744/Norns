using Microsoft.CodeAnalysis;
using Norns.Destiny.Structure;
using System.Collections.Generic;

namespace Norns.Skuld.Structure
{
    public static class AotSymbolExtensions
    {
        public static AccessibilityInfo ConvertToStructure(this Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    return AccessibilityInfo.Private;

                case Accessibility.ProtectedAndInternal:
                    return AccessibilityInfo.ProtectedAndInternal;

                case Accessibility.Protected:
                    return AccessibilityInfo.Protected;

                case Accessibility.Internal:
                    return AccessibilityInfo.Internal;

                case Accessibility.ProtectedOrInternal:
                    return AccessibilityInfo.ProtectedOrInternal;

                case Accessibility.Public:
                    return AccessibilityInfo.Public;

                default:
                    return AccessibilityInfo.NotApplicable;
            }
        }

        public static RefKindInfo ConvertToStructure(this VarianceKind variance)
        {
            switch (variance)
            {
                case VarianceKind.Out:
                    return RefKindInfo.Out;

                case VarianceKind.In:
                    return RefKindInfo.In;

                default:
                    return RefKindInfo.None;
            }
        }

        public static ISymbolInfo ConvertToStructure(this ISymbol symbol)
        {
            switch (symbol)
            {
                case IFieldSymbol f:
                    return new FieldSymbolInfo(f);

                case IMethodSymbol m:
                    return new MethodSymbolInfo(m);

                case IPropertySymbol p:
                    return new PropertySymbolInfo(p);

                default:
                    return null;
            }
        }

        public static RefKindInfo ConvertToStructure(this RefKind refKind)
        {
            switch (refKind)
            {
                case RefKind.Ref:
                    return RefKindInfo.Ref;

                case RefKind.In:
                    return RefKindInfo.In;

                case RefKind.Out:
                    return RefKindInfo.Out;

                default:
                    return RefKindInfo.None;
            }
        }

        public static MethodKindInfo ConvertToStructure(this MethodKind methodKind)
        {
            switch (methodKind)
            {
                case MethodKind.Ordinary:
                case MethodKind.DeclareMethod:
                    return MethodKindInfo.Method;

                case MethodKind.PropertyGet:
                    return MethodKindInfo.PropertyGet;

                case MethodKind.PropertySet:
                    return MethodKindInfo.PropertySet;

                case MethodKind.Constructor:
                    return MethodKindInfo.Constructor;

                case MethodKind.StaticConstructor:
                    return MethodKindInfo.StaticConstructor;

                case MethodKind.EventAdd:
                    return MethodKindInfo.EventAdd;

                case MethodKind.EventRemove:
                    return MethodKindInfo.EventRemove;

                case MethodKind.EventRaise:
                    return MethodKindInfo.EventRaise;

                default:
                    return MethodKindInfo.AnonymousFunction;
            }
        }

        public static ITypedConstantInfo ConvertToStructure(this TypedConstant constant)
        {
            return new TypedConstantInfo()
            {
                Type = new TypeSymbolInfo(constant.Type),
                Value = constant.Value
            };
        }

        public static INamedConstantInfo ConvertToStructure(this KeyValuePair<string, TypedConstant> constant)
        {
            return new TypedConstantInfo()
            {
                Type = new TypeSymbolInfo(constant.Value.Type),
                Value = constant.Value.Value,
                Name = constant.Key
            };
        }

        public static IAttributeSymbolInfo ConvertToStructure(this AttributeData attributeData)
        {
            return new AttributeSymbolInfo(attributeData);
        }
    }
}