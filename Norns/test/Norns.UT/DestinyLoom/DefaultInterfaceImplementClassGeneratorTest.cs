using Microsoft.CodeAnalysis;
using Norns.DestinyLoom;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Norns.UT.DestinyLoom
{
    public class DefaultInterfaceImplementClassTestGenerator : AbstractProxyGenerator
    {
        public override IEnumerable<IInterceptorGenerator> FindInterceptorGenerators()
        {
            return new IInterceptorGenerator[0];
        }

        public override bool CanProxy(INamedTypeSymbol @type)
        {
            return @type.TypeKind == TypeKind.Interface;
        }

        public override IEnumerable<AbstractProxyClassGenerator> FindProxyClassGenerators(IInterceptorGenerator[] interceptors)
        {
            yield return new DefaultInterfaceImplementClassGenerator(interceptors);
        }
    }

    public class DefaultInterfaceImplementClassGeneratorTest
    {
        private static Compilation GenerateSource(string source)
        {
            return ProxyGeneratorTest.GenerateSource(source, new DefaultInterfaceImplementClassTestGenerator());
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndVoidMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        void AddOne();
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public  void AddOne() {  }", str);
            Assert.Contains("[Norns.Fate.Abstraction.DefaultInterfaceImplement(typeof(Norns.ProxyGenerators.Test.IC))]", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndReturnIntMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        int AddOne(int v);
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public  int AddOne(int v)", str);
            Assert.Contains("return default;", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceHasDefaultMethodAndReturnIntMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        public int AddOne(int v)
        {
            return v + 1;
        }
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.DoesNotContain("AddOne", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndReturnValueTupleMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        (int,int) AddOne(int v);
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public  (int, int) AddOne(int v)", str);
            Assert.Contains("return default;", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndReturnValueListIntMethod()
        {
            var source = @"
using System.Collections.Generic;
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        List<int> AddOne(int v);
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("return default;", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndReturnValueTaskMethod()
        {
            var source = @"
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        Task AddOne(int v);
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("return System.Threading.Tasks.Task.CompletedTask;", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndReturnValueTaskIntMethod()
        {
            var source = @"
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        Task<int> AddOne(int v);
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("return System.Threading.Tasks.Task.FromResult<int>(default);", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceInheritInterfaceAndReturnValueTaskIntMethod()
        {
            var source = @"
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Norns.ProxyGenerators.Test
{
public interface ICD : System.IDisposable
    {
        Task<int> AddOne(int v);
    }
    public interface IC : ICD,System.IDisposable
    {
        Task<int> AddOne2(int v);
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.Select(i => i.ToString()).ToArray();
            Assert.Equal(2, array.Length);
            var str = array.First(i => i.Contains(": Norns.ProxyGenerators.Test.IC {"));
            Assert.Contains("Task<int> AddOne(int v)", str);
            Assert.Contains("Task<int> AddOne2(int v)", str);
            Assert.Contains("return System.Threading.Tasks.Task.FromResult<int>(default);", str);
        }

        [Fact]
        public void GenerateProxyClass()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        int AddOne(int v);
    }

    public class C : IC
    {
        public int AddOne(int v)
{
    return v + 1;
}
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public  int AddOne(int v)", str);
            Assert.Contains("return default;", str);
            Assert.DoesNotContain("class C", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndPropertyGetSetMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        int A { get; set; }
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public int A {  get;  set; }", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndPropertyNoGetMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        int A { set; }
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public int A {   set; }", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndInternalPropertyGetSetMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        int A { get; internal set; }
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public int A {  get; internal set; }", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndIndexerMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        string this[int a, string bb] { get; set; }
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public string this[int a,string bb] {  get { return default(string); }   set {   }  }", str);
        }
    }
}