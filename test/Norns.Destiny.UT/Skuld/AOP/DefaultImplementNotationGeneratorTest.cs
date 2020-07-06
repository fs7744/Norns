using Norns.Destiny.AOP;
using Norns.Destiny.AOP.Notations;
using Norns.Destiny.Loom;
using Norns.Skuld.AOP;
using System.Collections.Generic;
using Xunit;

namespace Norns.Destiny.UT.Skuld.AOP
{
    public class EmptyInterceptorGenerator : AbstractInterceptorGenerator
    { }

    public class DefaultImplementNotationGeneratorCollector : AopSourceGenerator
    {
        protected override IEnumerable<IInterceptorGenerator> GetInterceptorGenerators()
        {
            yield return new EmptyInterceptorGenerator();
        }

        protected override IEnumerable<INotationGenerator> CreateNotationGenerators()
        {
            yield return new DefaultImplementNotationGenerator(FilterForDefaultImplement);
        }
    }

    public class DefaultImplementNotationGeneratorTest
    {
        public string Generate(string code)
        {
            var collector = new DefaultImplementNotationGeneratorCollector();
            return SkuldTest.GenerateCode($"namespace Norns.Destiny.UT.AOT.Generated {{ {code} }}", collector);
        }

        #region Interface

        [Fact]
        public void WhenSimpleInterfaceSyncMethodAndHasReturnValue()
        {
            var code = @"
using Norns.Destiny.Attributes;
    [Charon]
    public interface IC
    {
        int AddOne(int v);

        int PA { get; set; }

        int PD { set; }

        string this[int v, string s] { get; set; }
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
            Assert.Contains("public int AddOne(int v)", output);
            Assert.Contains("= default(int);return", output);
            Assert.Contains("public int PA", output);
            Assert.Contains("public int PD", output);
            Assert.Contains("public string this[int v,string s]", output);
        }

        [Fact]
        public void WhenSimpleInterfaceSyncMethodAndVoid()
        {
            var code = @"
using Norns.Destiny.Attributes;
    [Charon]
    public interface IC
    {
        void AddVoid();
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
            Assert.Contains("public void AddVoid()", output);
            Assert.DoesNotContain("return", output);
        }

        [Fact]
        public void WhenSimpleInterfaceAsyncMethodAndVoid()
        {
            var code = @"
using System.Threading.Tasks;
using Norns.Destiny.Attributes;
    [Charon]
    public interface IC
    {
        Task AddTask(int v);
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
            Assert.Contains("public async System.Threading.Tasks.Task AddTask(int v)", output);
            Assert.DoesNotContain("return", output);
        }

        [Fact]
        public void WhenSimpleInterfaceAsyncMethodAndTaskInt()
        {
            var code = @"
using System.Threading.Tasks;

using Norns.Destiny.Attributes;
    [Charon]public interface IC
    {
        Task<int> AddVTask(int v);
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
            Assert.Contains("public async System.Threading.Tasks.Task<int> AddVTask(int v)", output);
            Assert.Contains("= default(int);return", output);
        }

        [Fact]
        public void WhenSimpleInterfaceAsyncMethodAndValueTaskInt()
        {
            var code = @"
using System.Threading.Tasks;

using Norns.Destiny.Attributes;
    [Charon]public interface IC
    {
        ValueTask<int> AddValueTask(int v);
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
            Assert.Contains("public async System.Threading.Tasks.ValueTask<int> AddValueTask(int v)", output);
            Assert.Contains("= default(int);return", output);
        }

        [Fact]
        public void WhenSimpleInterfaceAsyncMethodAndValueTaskT()
        {
            var code = @"
using System.Threading.Tasks;

using Norns.Destiny.Attributes;
    [Charon]public interface IC
    {
        ValueTask<T> AddValueTask<T>(T v);
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
            Assert.Contains("public async System.Threading.Tasks.ValueTask<T> AddValueTask<T>(T v)", output);
            Assert.Contains("= default(T);return", output);
        }

        [Fact]
        public void WhenSimpleInterfaceAsyncMethodAndValueTaskTV()
        {
            var code = @"
using System.Threading.Tasks;

using Norns.Destiny.Attributes;
    [Charon]public interface IC
    {
        ValueTask<Task<T>> AddValueTask<T,V>(T v,V v1) where T : struct where V : class, IC;
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
            Assert.Contains("public async System.Threading.Tasks.ValueTask<System.Threading.Tasks.Task<T>> AddValueTask<T,V>(T v,V v1)where T : struct where V : class,Norns.Destiny.UT.AOT.Generated.IC", output);
            Assert.Contains("= default(System.Threading.Tasks.Task<T>);return", output);
        }

        [Fact]
        public void WhenSimpleInterfaceAsyncMethodAndValueTaskTVAndRefKind()
        {
            var code = @"
using System.Threading.Tasks;

using Norns.Destiny.Attributes;
    [Charon]public interface IC
    {
        ValueTask<Task<T>> AddValueTask<T,V>(T v,ref V v1);
ValueTask<Task<T>> AddValueTask2<T,V>(T v,in V v1);
ValueTask<Task<T>> AddValueTask3<T,V>(T v,out V v3);
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
            Assert.Contains("public async System.Threading.Tasks.ValueTask<System.Threading.Tasks.Task<T>> AddValueTask<T,V>(T v,ref V v1)", output);
            Assert.Contains("public async System.Threading.Tasks.ValueTask<System.Threading.Tasks.Task<T>> AddValueTask2<T,V>(T v,in V v1)", output);
            Assert.Contains("public async System.Threading.Tasks.ValueTask<System.Threading.Tasks.Task<T>> AddValueTask3<T,V>(T v,out V v3)", output);
            Assert.DoesNotContain("where", output);
            Assert.Contains("v3 = default;", output);
            Assert.Contains("= default(System.Threading.Tasks.Task<T>);return", output);
        }

        [Fact]
        public void WhenGenericInterfaceSyncMethod()
        {
            var code = @"

using Norns.Destiny.Attributes;
    [Charon]public interface IC<T> where T : class
    {
        T A();
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC<>))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains("<T>:Norns.Destiny.UT.AOT.Generated.IC<T> where T : class {", output);
            Assert.Contains("public T A()", output);
            Assert.Contains("= default(T);return", output);
        }

        [Fact]
        public void WhenOutGenericInterfaceSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;
    [Charon]
    public interface IC<out T> where T : class
    {
        T A();
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC<>))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains("<T>:Norns.Destiny.UT.AOT.Generated.IC<T> where T : class {", output);
            Assert.Contains("public T A()", output);
            Assert.Contains("= default(T);return", output);
        }

        [Fact]
        public void WhenInGenericInterfaceSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;
    public class A {}
    public class B : A {}
    [Charon]
    public interface IC<in T, V, R> where T : A
    {
        B A();
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC<,,>))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains("<T,V,R>:Norns.Destiny.UT.AOT.Generated.IC<T, V, R> where T : Norns.Destiny.UT.AOT.Generated.A", output);
            Assert.Contains("public Norns.Destiny.UT.AOT.Generated.B A()", output);
            Assert.Contains("= default(Norns.Destiny.UT.AOT.Generated.B);return", output);
        }

        [Fact]
        public void WhenInGenericNestedInterfaceSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;
    public class A {
    [Charon]
    public interface IC<R>
    {
        R A();
    } }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.A.IC<>))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains("<R>:Norns.Destiny.UT.AOT.Generated.A.IC<R>", output);
            Assert.Contains("public R A()", output);
            Assert.Contains("= default(R);return", output);
        }

        [Fact]
        public void WhenInGenericNestedNestedInterfaceSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;
public class B {
    public class A {
    [Charon]
    public interface IC<R>
    {
        R A();
    } } }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.B.A.IC<>))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains("<R>:Norns.Destiny.UT.AOT.Generated.B.A.IC<R>", output);
            Assert.Contains("public R A()", output);
            Assert.Contains("= default(R);return", output);
        }

        [Fact]
        public void WhenInterfaceHasDefaultSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;
    [Charon]
    public interface IC
    {
        public int A() => 3;
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
            Assert.DoesNotContain("public int A()", output);
            Assert.DoesNotContain("return default;", output);
        }

        [Fact]
        public void WhenInterfaceHasInhertSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;

public interface ID
{
    void B();
}

    [Charon]
    public interface IC : ID
    {
        public int A() => 3;
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
            Assert.DoesNotContain("public int A()", output);
            Assert.Contains("public void B()", output);
            Assert.DoesNotContain("return default;", output);
        }

        #endregion Interface

        #region Abstract Class

        [Fact]
        public void WhenAbstractClassSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;
using System.Threading.Tasks;

[Charon]
    public abstract class JitCClass
    {
        public abstract int AddOne(int v);

        public abstract void AddVoid();

        public abstract Task AddTask(int v);

        public abstract Task<int> AddVTask(int v);

        public abstract ValueTask<int> AddValueTask(int v);

        public abstract ValueTask<T> AddValueTask<T>(T v);

        public abstract ValueTask<Task<T>> AddValueTask<T, V>(T v, V v1) where T : struct where V : JitCClass;

        public abstract IEnumerable<T> AddValue1<T, V>(T v, ref V v1);

        public abstract IEnumerable<T> AddValue2<T, V>(T v, in V v1);

        public abstract IEnumerable<T> AddValue3<T, V>(T v, out V v1);

        public int A() => 3;

        public virtual int B() => 3;

        public virtual int PA { get; set; }

        public virtual int PD { protected get; set; }

        public abstract string this[int v, string s] { get;set; }
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.JitCClass))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.JitCClass {", output);
            Assert.Contains("public override int AddOne(int v)", output);
            Assert.Contains("public override void AddVoid()", output);
            Assert.Contains("public async override System.Threading.Tasks.Task AddTask(int v)", output);
            Assert.Contains("public async override System.Threading.Tasks.Task<int> AddVTask(int v)", output);
            Assert.Contains("public async override System.Threading.Tasks.ValueTask<int> AddValueTask(int v)", output);
            Assert.Contains("public async override System.Threading.Tasks.ValueTask<T> AddValueTask<T>(T v)", output);
            Assert.Contains("public async override System.Threading.Tasks.ValueTask<System.Threading.Tasks.Task<T>> AddValueTask<T,V>(T v,V v1)where T : struct", output);
            Assert.Contains("public override IEnumerable<T> AddValue1<T,V>(T v,ref V v1)", output);
            Assert.Contains("public override IEnumerable<T> AddValue2<T,V>(T v,in V v1)", output);
            Assert.Contains("public override IEnumerable<T> AddValue3<T,V>(T v,out V v1)", output);
            Assert.DoesNotContain("B()", output);
            Assert.DoesNotContain("A()", output);
            Assert.Contains("public override int PA", output);
            Assert.Contains("public override int PD", output);
            Assert.Contains("public override string this[int v,string s]", output);
        }

        [Fact]
        public void WhenGenericAbstractClassSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;
using System.Threading.Tasks;

[Charon]
    public abstract class JitCClass<T> where T : class
    {
        public abstract T B();
    }";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.JitCClass<>))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains("<T>:Norns.Destiny.UT.AOT.Generated.JitCClass<T> where T : class {", output);
            Assert.Contains("public override T B()", output);
        }

        [Fact]
        public void WhenNestedGenericAbstractClassSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;
using System.Threading.Tasks;

 public class A {
[Charon]
    public abstract class JitCClass<T> where T : class
    {
        public abstract T B();
    }}";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.A.JitCClass<>))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains("<T>:Norns.Destiny.UT.AOT.Generated.A.JitCClass<T> where T : class {", output);
            Assert.Contains("public override T B()", output);
        }

        [Fact]
        public void WhenNestedNestedGenericAbstractClassSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;
using System.Threading.Tasks;
public class A {
 public class A {
[Charon]
    public abstract class JitCClass<T> where T : class
    {
        public abstract T B();
    }}}";
            var output = Generate(code);
            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.A.A.JitCClass<>))]", output);
            Assert.Contains("public class DefaultImplement", output);
            Assert.Contains("<T>:Norns.Destiny.UT.AOT.Generated.A.A.JitCClass<T> where T : class {", output);
            Assert.Contains("public override T B()", output);
        }

        #endregion Abstract Class
    }
}