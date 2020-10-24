using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Norns.Urd.Proxy
{
    public class FacadeProxyGenerator : AbstractProxyGenerator
    {
        public override ProxyTypes ProxyType { get; } = ProxyTypes.Facade;

        public override void DefineFields(TypeBuilder typeBuilder, Type serviceType)
        {
            typeBuilder.DefineField("instance", serviceType, FieldAttributes.Private);
        }
    }
}