using Microsoft.CodeAnalysis;
using Norns.DestinyLoom;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Norns.UT.DestinyLoom
{
    public class ClassProxyClassTestGenerator : AbstractProxyGenerator
    {
        public override IEnumerable<IInterceptorGenerator> FindInterceptorGenerators()
        {
            return new IInterceptorGenerator[0];
        }

        public override bool CanProxy(INamedTypeSymbol @type)
        {
            return @type.TypeKind == TypeKind.Class;
        }

        public override IEnumerable<AbstractProxyClassGenerator> FindProxyClassGenerators(IInterceptorGenerator[] interceptors)
        {
            yield return new ClassProxyClassGenerator(interceptors);
        }
    }

    public class ClassProxyClassGeneratorTest
    {
        private static Compilation GenerateSource(string source)
        {
            return ProxyGeneratorTest.GenerateSource(source, new ClassProxyClassTestGenerator());
        }

        [Fact]
        public void GenerateProxyClassWhenClassAndNoInheritMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public class C
    {
        public void AddOne() {};
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.C", str);
            Assert.DoesNotContain("AddOne()", str);
            Assert.Contains("[Norns.Fate.Abstraction.Proxy(typeof(Norns.ProxyGenerators.Test.C))]", str);
            Assert.Contains("proxy", str);
        }

        [Fact]
        public void GenerateProxyClassWhenClassAndVoidMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public class C
    {
        public abstract void AddOne();
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.C", str);
            Assert.Contains("public override  void AddOne() {  }", str);
            Assert.Contains("[Norns.Fate.Abstraction.Proxy(typeof(Norns.ProxyGenerators.Test.C))]", str);
            Assert.Contains("proxy", str);
        }

        [Fact]
        public void GenerateProxyClassWhenAbstractClassAndVoidMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public abstract class C
    {
        public abstract void AddOne();
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.C", str);
            Assert.Contains("public override  void AddOne() {  } ", str);
            Assert.Contains("[Norns.Fate.Abstraction.Proxy(typeof(Norns.ProxyGenerators.Test.C))]", str);
            Assert.Contains("proxy", str);
        }

        [Fact]
        public void GenerateProxyClassWhenClassAndVirtualMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public class C
    {
        public virtual int AddOne() => 1;
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.C", str);
            Assert.Contains("public override  int AddOne()", str);
            Assert.Contains("[Norns.Fate.Abstraction.Proxy(typeof(Norns.ProxyGenerators.Test.C))]", str);
            Assert.Contains("= proxy", str);
            Assert.Contains(".AddOne();", str);
            Assert.Contains("= default(int)", str);
            Assert.Contains("return r", str);
        }
         
        [Fact]
        public void GenerateProxyClassWhenClassAndOverrideMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public class C
    {
        public virtual int AddOne() => 1;
    }
    public class Cd : C
    {
        public override int AddOne() => 2;
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyCd", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.Cd", str);
            Assert.Contains("public override  int AddOne()", str);
            Assert.Contains("[Norns.Fate.Abstraction.Proxy(typeof(Norns.ProxyGenerators.Test.Cd))]", str);
            Assert.Contains("= proxy", str);
            Assert.Contains(".AddOne();", str);
            Assert.Contains("= default(int)", str);
            Assert.Contains("return r", str);
        }

        [Fact]
        public void GenerateProxyClassWhenClassWithAsyncMethod()
        {
            var source = @"
using System.Threading.Tasks;
namespace Norns.ProxyGenerators.Test
{
        public class C
    {
        async virtual Task AddOne(int v) {}
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.C", str);
            Assert.Contains("private async override  System.Threading.Tasks.Task AddOne(int v)", str);
            Assert.Contains("await proxy", str);
            Assert.Contains(".AddOne(v);", str);
            Assert.DoesNotContain("return r", str);
        }

        [Fact]
        public void GenerateProxyClassWhenClassWithAsyncValueMethod()
        {
            var source = @"
using System.Threading.Tasks;
namespace Norns.ProxyGenerators.Test
{
    public  class C
    {
        virtual Task<int> AddOne(int v) {}
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.C", str);
            Assert.Contains("private async override  System.Threading.Tasks.Task<int> AddOne(int v)", str);
            Assert.Contains("= await proxy", str);
            Assert.Contains(".AddOne(v);", str);
            Assert.Contains("= default(int);", str);
            Assert.Contains("return r", str);
        }

        [Fact]
        public void GenerateProxyClassWhenClassWithAsyncValueValueTaskMethod()
        {
            var source = @"
using System.Threading.Tasks;
namespace Norns.ProxyGenerators.Test
{
    public class C
    {
        protected virtual ValueTask<int> AddOne(int v) {}
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.C", str);
            Assert.Contains("protected async override  System.Threading.Tasks.ValueTask<int> AddOne(int v)", str);
            Assert.Contains("= await proxy", str);
            Assert.Contains(".AddOne(v);", str);
            Assert.Contains("= default(int);", str);
            Assert.Contains("return r", str);
        }

        [Fact]
        public void GenerateProxyClassWhenClassWithIntPropertyGetSetMethod()
        {
            var source = @"
using System.Threading.Tasks;
namespace Norns.ProxyGenerators.Test
{
    public class C
    {
        public virtual int V {get;set;}
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.C", str);
            Assert.Contains("public override int V {", str);
            Assert.Contains("= default(int);", str);
            Assert.Contains("get", str);
            Assert.Contains(".V;", str);
            Assert.Contains("return r", str);
            Assert.Contains("set", str);
            Assert.Contains("= value;", str);
            Assert.Contains(".V =", str);
        }

        [Fact]
        public void GenerateProxyClassWhenClassWithPropertyInternalGetSetMethod()
        {
            var source = @"
using System.Threading.Tasks;
namespace Norns.ProxyGenerators.Test
{
    public class C
    {
        public virtual object V { internal get; internal set;}
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.C", str);
            Assert.Contains("public override object V {", str);
            Assert.Contains("= default(object);", str);
            Assert.Contains("internal get", str);
            Assert.Contains(".V;", str);
            Assert.Contains("return r", str);
            Assert.Contains("internal set", str);
            Assert.Contains("= value;", str);
            Assert.Contains(".V =", str);
        }

        [Fact]
        public void GenerateProxyClassWhenClassAndIndexerMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public class C
    {
        public virtual string this[int a, string bb] { get; set; }
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.C", str);
            Assert.Contains("public override string this[int a,string bb] {", str);
            Assert.Contains("= default(string);", str);
            Assert.Contains("get", str);
            Assert.Contains("[a,bb];", str);
            Assert.Contains("return r", str);
            Assert.Contains("set", str);
            Assert.Contains("= value;", str);
            Assert.Contains("[a,bb] =", str);
        }

        [Fact]
        public void GenerateProxyClassWhenClassAndConstructorMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public class C
    {
        public C() {}
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.C", str);
            Assert.Contains("public   ProxyC", str);
            Assert.Contains("() : base()  {  }", str);
        }
    }
}