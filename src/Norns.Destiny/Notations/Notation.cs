using Norns.Destiny.Structure;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.Notations
{
    public static class Notation
    {
        public static INotation Combine(this IEnumerable<INotation> notations)
        {
            return new ActionNotation(sb =>
            {
                foreach (var item in notations)
                {
                    item.Record(sb);
                }
            });
        }

        public static IEnumerable<INotation> InsertComma(this IEnumerable<INotation> notations)
        {
            return notations.InsertSeparator(ConstNotations.Comma);
        }

        public static IEnumerable<INotation> InsertBlank(this IEnumerable<INotation> notations)
        {
            return notations.InsertSeparator(ConstNotations.Blank);
        }

        #region ToNotation

        public static INotation ToNotation(this string value)
        {
            return new StringNotation(value);
        }

        public static TypeParameterNotation ToNotation(this ITypeParameterSymbolInfo value)
        {
            var notation = new TypeParameterNotation()
            {
                RefKind = value.RefKind,
                Type = value.FullName,
                Name = value.Name
            };

            if (value.HasReferenceTypeConstraint)
            {
                notation.Constants.Add(ConstNotations.Class);
            }

            if (value.HasValueTypeConstraint)
            {
                notation.Constants.Add(ConstNotations.Struct);
            }
            notation.Constants.AddRange(value.ConstraintTypes.Select(i => i.FullName.ToNotation()));
            if (!value.HasValueTypeConstraint && value.HasConstructorConstraint)
            {
                notation.Constants.Add(ConstNotations.ConstructorConstraint);
            }
            return notation;
        }

        public static ConstructorNotation ToConstructorNotation(this IMethodSymbolInfo method, string className)
        {
            var notation = new ConstructorNotation()
            {
                Name = className,
                Accessibility = method.Accessibility
            };
            notation.BaseParameters.AddRange(method.Parameters.Select(i => new ParameterNotation()
            {
                RefKind = i.RefKind,
                Type = i.Type.FullName,
                Name = i.Name
            }));
            return notation;
        }

        public static MethodNotation ToNotationDefinition(this IMethodSymbolInfo method)
        {
            var notation = new MethodNotation()
            {
                Accessibility = method.Accessibility,
                ReturnType = method.ReturnType.FullName,
                Name = method.Name
            };
            notation.Parameters.AddRange(method.Parameters.Select(i => new ParameterNotation()
            {
                RefKind = i.RefKind,
                Type = i.Type.FullName,
                Name = i.Name
            }));
            if (method.IsGenericMethod)
            {
                notation.TypeParameters.AddRange(method.TypeParameters.Select(i => i.ToNotation()));
            }
            notation.IsAsync = method.IsAsync;
            return notation;
        }

        public static INotation ToCallParameters(this IEnumerable<ParameterNotation> values)
        {
            return values.Select(i => i.ToCallParameter()).InsertComma().Combine();
        }

        public static IEnumerable<INotation> ToNotations(this IEnumerable<string> values)
        {
            return values.Select(i => i.ToNotation());
        }

        public static IEnumerable<INotation> Create(params string[] values)
        {
            return ToNotations(values);
        }

        #endregion ToNotation
    }
}