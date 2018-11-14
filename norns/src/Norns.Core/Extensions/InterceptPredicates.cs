using Norns.AOP.Extensions;
using Norns.AOP.Interceptors;
using System.Collections.Generic;

namespace Norns.AOP.Configuration
{
    public static class InterceptPredicates
    {
        public static InterceptPredicate ForNamespace(string nameSpace)
        {
            Arguments.NotNullOrEmpty(nameof(nameSpace), nameSpace);
            return method => method.DeclaringType.Namespace != null && method.DeclaringType.Namespace.Matches(nameSpace);
        }

        public static InterceptPredicate ForService(string service)
        {
            Arguments.NotNullOrEmpty(nameof(service), service);
            return method =>
            {
                if (method.DeclaringType.Name.Matches(service))
                {
                    return true;
                }
                var declaringType = method.DeclaringType;
                var fullName = declaringType.FullName ?? $"{declaringType.Name}.{declaringType.Name}";
                return fullName.Matches(service);
            };
        }

        public static InterceptPredicate ForMethod(string method)
        {
            Arguments.NotNullOrEmpty(nameof(method), method);
            return methodInfo => methodInfo.Name.Matches(method);
        }

        public static InterceptPredicate ForMethod(string service, string method)
        {
            Arguments.NotNullOrEmpty(nameof(service), service);
            Arguments.NotNullOrEmpty(nameof(method), method);
            return methodInfo => ForService(service)(methodInfo) && methodInfo.Name.Matches(method);
        }

        public static IList<InterceptPredicate> AddNamespace(this IList<InterceptPredicate> lists, string nameSpace)
        {
            lists.Add(ForNamespace(nameSpace));
            return lists;
        }

        public static IList<InterceptPredicate> AddService(this IList<InterceptPredicate> lists, string service)
        {
            lists.Add(ForService(service));
            return lists;
        }

        public static IList<InterceptPredicate> AddMethod(this IList<InterceptPredicate> lists, string method)
        {
            lists.Add(ForMethod(method));
            return lists;
        }

        public static IList<InterceptPredicate> AddMethod(this IList<InterceptPredicate> lists, string service, string method)
        {
            lists.Add(ForMethod(service, method));
            return lists;
        }

        internal static IList<InterceptPredicate> AddDefaultBlacklists(this IList<InterceptPredicate> lists)
        {
            lists.AddNamespace("System");
            lists.AddNamespace("System.*");
            lists.AddNamespace("Microsoft.*");
            lists.AddNamespace("Microsoft.CodeAnalysis.Razor");
            lists.AddNamespace("Microsoft.CodeAnalysis.Razor.*");
            lists.AddNamespace("Microsoft.AspNetCore.*");
            lists.AddNamespace("Microsoft.AspNetCore.Razor.Language");
            lists.AddNamespace("Microsoft.AspNet.*");
            lists.AddNamespace("Microsoft.Extensions.*");
            lists.AddNamespace("Microsoft.ApplicationInsights.*");
            lists.AddNamespace("Microsoft.Net.*");
            lists.AddNamespace("Microsoft.Web.*");
            lists.AddNamespace("Microsoft.Data.*");
            lists.AddNamespace("Microsoft.EntityFrameworkCore");
            lists.AddNamespace("Microsoft.EntityFrameworkCore.*");
            lists.AddNamespace("Microsoft.Owin.*");
            lists.AddNamespace("Owin");
            lists.AddMethod("Equals");
            lists.AddMethod("GetHashCode");
            lists.AddMethod("ToString");
            lists.AddMethod("GetType");
            lists.AddMethod("Finalize");
            lists.Add(m => m.DeclaringType == typeof(object));
            lists.AddNamespace("IdentityServer4");
            lists.AddNamespace("IdentityServer4.*");
            lists.AddNamespace("Norns");
            lists.AddNamespace("Norns.*");
            return lists;
        }
    }
}