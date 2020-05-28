﻿using Norns.AOP.Attributes;
using Norns.AOP.Interceptors;
using Norns.Extensions.Reflection;
using System;
using System.Linq;

namespace Norns.Core.AOP.Configuration
{
    [NoIntercept]
    public class TypeInterceptorCreator : InterceptorCreator
    {
        public TypeInterceptorCreator(Type interceptorType,
            InterceptPredicate whitelists, InterceptPredicate blacklists) : base(interceptorType, whitelists, blacklists)
        {
            if (!typeof(IInterceptor).IsAssignableFrom(interceptorType))
            {
                throw new ArgumentException("Not IInterceptor type.", interceptorType.FullName);
            }
        }

        public override IInterceptor Create(IServiceProvider serviceProvider)
        {
            var constructor = InterceptorType.GetConstructors().FirstOrDefault();
            if (constructor == null)
            {
                throw new ArgumentNullException(InterceptorType.FullName, "No found ctor.");
            }
            return (IInterceptor)constructor.GetReflector().InvokerFromService(serviceProvider);
        }
    }
}