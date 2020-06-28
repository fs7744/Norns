using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOP;
using Norns.Destiny.AOP.Notations;
using Norns.Destiny.Attributes;
using Norns.Destiny.JIT.AOP;
using Norns.Destiny.JIT.Coder;
using Norns.Destiny.JIT.Structure;
using Norns.Destiny.UT.AOT.AOP;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Norns.Destiny.UT.JIT.AOP
{
    public class OnlyDefaultImplementNotationGenerator : JitAopSourceGenerator
    {
        public OnlyDefaultImplementNotationGenerator(JitOptions options, IEnumerable<IInterceptorGenerator> generators) : base(options, generators)
        {
        }

        protected override IEnumerable<INotationGenerator> CreateNotationGenerators()
        {
            yield return new DefaultImplementNotationGenerator(options.FilterForDefaultImplement);
        }
    }

    public static class JitTest
    {
        private static JitOptions options = JitOptions.CreateDefault();

        public static Dictionary<string, TypeSymbolInfo> Generate(params Type[] types)
        {
            var generator = new OnlyDefaultImplementNotationGenerator(options, new IInterceptorGenerator[] { new EmptyInterceptorGenerator() });
            var assembly = generator.Generate(new JitTypesSymbolSource(types));
            return assembly.GetTypes().Select(i => new TypeSymbolInfo(i)).Where(i => i.HasAttribute<DefaultImplementAttribute>())
                .ToDictionary(i => i.Name, i => i);
        }
    }

    [Charon]
    public interface IJitC
    {
        int AddOne(int v);
    }

    public class DefaultImplementNotationGeneratorTest
    {
        [Fact]
        public void WhenSimpleInterfaceSyncMethodAndHasReturnValue()
        {
            var types = JitTest.Generate(typeof(IJitC));
            Assert.Single(types);
            var t = types.Values.First();
            Assert.True( t.GetAttributes().Any(i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(IJitC)));
            Assert.Equal(0, (Activator.CreateInstance(t.RealType) as IJitC).AddOne(33));
        }

//        [Fact]
//        public void WhenSimpleInterfaceSyncMethodAndVoid()
//        {
//            var code = @"
//    public interface IC
//    {
//        void AddVoid();
//    }";
//            var output = Generate(code);
//            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
//            Assert.Contains("public class DefaultImplement", output);
//            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
//            Assert.Contains("public void AddVoid()", output);
//            Assert.DoesNotContain("return", output);
//        }

//        [Fact]
//        public void WhenSimpleInterfaceAsyncMethodAndVoid()
//        {
//            var code = @"
//using System.Threading.Tasks;
//    public interface IC
//    {
//        Task AddTask(int v);
//    }";
//            var output = Generate(code);
//            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
//            Assert.Contains("public class DefaultImplement", output);
//            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
//            Assert.Contains("public async System.Threading.Tasks.Task AddTask(int v)", output);
//            Assert.DoesNotContain("return", output);
//        }

//        [Fact]
//        public void WhenSimpleInterfaceAsyncMethodAndTaskInt()
//        {
//            var code = @"
//using System.Threading.Tasks;
//    public interface IC
//    {
//        Task<int> AddVTask(int v);
//    }";
//            var output = Generate(code);
//            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
//            Assert.Contains("public class DefaultImplement", output);
//            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
//            Assert.Contains("public async System.Threading.Tasks.Task<int> AddVTask(int v)", output);
//            Assert.Contains("return default;", output);
//        }

//        [Fact]
//        public void WhenSimpleInterfaceAsyncMethodAndValueTaskInt()
//        {
//            var code = @"
//using System.Threading.Tasks;
//    public interface IC
//    {
//        ValueTask<int> AddValueTask(int v);
//    }";
//            var output = Generate(code);
//            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
//            Assert.Contains("public class DefaultImplement", output);
//            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
//            Assert.Contains("public async System.Threading.Tasks.ValueTask<int> AddValueTask(int v)", output);
//            Assert.Contains("return default;", output);
//        }

//        [Fact]
//        public void WhenSimpleInterfaceAsyncMethodAndValueTaskT()
//        {
//            var code = @"
//using System.Threading.Tasks;
//    public interface IC
//    {
//        ValueTask<T> AddValueTask<T>(T v);
//    }";
//            var output = Generate(code);
//            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
//            Assert.Contains("public class DefaultImplement", output);
//            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
//            Assert.Contains("public async System.Threading.Tasks.ValueTask<T> AddValueTask<T>(T v)", output);
//            Assert.Contains("return default;", output);
//        }

//        [Fact]
//        public void WhenSimpleInterfaceAsyncMethodAndValueTaskTV()
//        {
//            var code = @"
//using System.Threading.Tasks;
//    public interface IC
//    {
//        ValueTask<Task<T>> AddValueTask<T,V>(T v,V v1) where T : struct where V : class, IC;
//    }";
//            var output = Generate(code);
//            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
//            Assert.Contains("public class DefaultImplement", output);
//            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
//            Assert.Contains("public async System.Threading.Tasks.ValueTask<System.Threading.Tasks.Task<T>> AddValueTask<T,V>(T v,V v1)", output);
//            Assert.DoesNotContain("where", output);
//            Assert.Contains("return default;", output);
//        }

//        [Fact]
//        public void WhenSimpleInterfaceAsyncMethodAndValueTaskTVAndRefKind()
//        {
//            var code = @"
//using System.Threading.Tasks;
//    public interface IC
//    {
//        ValueTask<Task<T>> AddValueTask<T,V>(T v,ref V v1);
//ValueTask<Task<T>> AddValueTask2<T,V>(T v,in V v1);
//ValueTask<Task<T>> AddValueTask3<T,V>(T v,out V v1);
//    }";
//            var output = Generate(code);
//            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC))]", output);
//            Assert.Contains("public class DefaultImplement", output);
//            Assert.Contains(":Norns.Destiny.UT.AOT.Generated.IC {", output);
//            Assert.Contains("public async System.Threading.Tasks.ValueTask<System.Threading.Tasks.Task<T>> AddValueTask<T,V>(T v,ref V v1)", output);
//            Assert.Contains("public async System.Threading.Tasks.ValueTask<System.Threading.Tasks.Task<T>> AddValueTask2<T,V>(T v,in V v1)", output);
//            Assert.Contains("public async System.Threading.Tasks.ValueTask<System.Threading.Tasks.Task<T>> AddValueTask3<T,V>(T v,out V v1)", output);
//            Assert.DoesNotContain("where", output);
//            Assert.Contains("return default;", output);
//        }

//        [Fact]
//        public void WhenGenericInterfaceSyncMethod()
//        {
//            var code = @"
//    public interface IC<T> where T : class
//    {
//        T A();
//    }";
//            var output = Generate(code);
//            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC<>))]", output);
//            Assert.Contains("public class DefaultImplement", output);
//            Assert.Contains("<T>:Norns.Destiny.UT.AOT.Generated.IC<T>where T : class {", output);
//            Assert.Contains("public T A()", output);
//            Assert.Contains("return default;", output);
//        }

//        [Fact]
//        public void WhenOutGenericInterfaceSyncMethod()
//        {
//            var code = @"
//    public interface IC<out T> where T : class
//    {
//        T A();
//    }";
//            var output = Generate(code);
//            Assert.Contains("[Norns.Destiny.Attributes.DefaultImplement(typeof(Norns.Destiny.UT.AOT.Generated.IC<>))]", output);
//            Assert.Contains("public class DefaultImplement", output);
//            Assert.Contains("<T>:Norns.Destiny.UT.AOT.Generated.IC<T>where T : class {", output);
//            Assert.Contains("public T A()", output);
//            Assert.Contains("return default;", output);
//        }
    }
}