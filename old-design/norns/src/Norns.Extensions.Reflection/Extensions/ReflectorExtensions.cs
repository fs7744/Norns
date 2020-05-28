using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Norns.Extensions.Reflection
{
    public static class ReflectorExtensions
    {
        public static TypeReflector GetReflector(this Type type)
        {
            return ReflectorCache<Type, TypeReflector>.GetOrAdd(type, t => new TypeReflector(t));
        }

        public static MethodReflector GetReflector(this MethodInfo method)
        {
            return ReflectorCache<MethodInfo, MethodReflector>.GetOrAdd(method, t => new MethodReflector(t));
        }

        public static ConstructorReflector GetReflector(this ConstructorInfo constructorInfo)
        {
            return ReflectorCache<ConstructorInfo, ConstructorReflector>.GetOrAdd(constructorInfo, t => new ConstructorReflector(t));
        }

        public static ParameterReflector GetReflector(this ParameterInfo parameterInfo)
        {
            return ReflectorCache<ParameterInfo, ParameterReflector>.GetOrAdd(parameterInfo, t => new ParameterReflector(t));
        }

        public static PropertyReflector GetReflector(this PropertyInfo propertyInfo)
        {
            return ReflectorCache<PropertyInfo, PropertyReflector>.GetOrAdd(propertyInfo, t => new PropertyReflector(t));
        }

        public static MethodInfo GetMethod<T>(Expression<T> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            if (!(expression.Body is MethodCallExpression methodCallExpression))
            {
                throw new InvalidCastException("Cannot be converted to MethodCallExpression");
            }
            return methodCallExpression.Method;
        }
    }
}