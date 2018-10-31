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

        public static MethodInfo GetMethod<T>(Expression<T> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression == null)
            {
                throw new InvalidCastException("Cannot be converted to MethodCallExpression");
            }
            return methodCallExpression.Method;
        }
    }
}