using Norns.AOP.Attributes;
using Norns.AOP.Codegen;
using Norns.AOP.Interceptors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Norns.Test.Codegen
{
    public class ProxyCodeGeneratorTest
    {
        [Fact]
        public void Test()
        {
            var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.ToString();
            new ProxyCodeGenerator()
            {
                SrcDirectory = projectDirectory,
                OutputDirectory = Path.Combine(projectDirectory, "obj"),
            }.Generate();
        }

        public class AddOne2Attribute : InterceptorBaseAttribute
        {
            public override async Task InterceptAsync(InterceptContext context, AsyncInterceptDelegate nextAsync)
            {
                await nextAsync(context);
                context.Result = (int)context.Result + 1;
            }
        }

        public static class A
        {

        }

        public class AB
        {
            public AB(int a, string b)
            {
                A = a;
                B = b;
            }

            public int A { get; }
            public string B { get; }

            [AddOne2]
            public virtual void AA(int a, string b)
            {
                //Norns.AOP
            }

            public virtual async Task BB(int a, string b)
            {

            }
        }
    }
}
