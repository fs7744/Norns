using System;
using System.Reflection.Emit;

namespace Norns.Urd.Proxy
{
    public interface IProxyGenerator
    {
        ProxyTypes ProxyType { get; }

        string GetProxyTypeName(Type serviceType);

        Type CreateProxyType(string proxyTypeName, Type serviceType, ModuleBuilder moduleBuilder);
    }
}