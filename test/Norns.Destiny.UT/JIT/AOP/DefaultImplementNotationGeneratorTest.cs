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
using System.Threading.Tasks;
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

        void AddVoid(); 

        Task AddTask(int v);

        Task<int> AddVTask(int v); 
        
        ValueTask<int> AddValueTask(int v);

        ValueTask<T> AddValueTask<T>(T v);
    }

    public class DefaultImplementNotationGeneratorTest
    {
        [Fact]
        public async Task WhenSimpleInterfaceAndSomeMethods()
        {
            var types = JitTest.Generate(typeof(IJitC));
            Assert.Single(types);
            var t = types.Values.First();
            Assert.True( t.GetAttributes().Any(i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(IJitC)));
            var instance = Activator.CreateInstance(t.RealType) as IJitC;
            Assert.Equal(0, instance.AddOne(33));
            instance.AddVoid();
            await instance.AddTask(66);
            Assert.Equal(0, await instance.AddVTask(44));
            Assert.Equal(0, await instance.AddValueTask(11));
            Assert.Null(await instance.AddValueTask(this));
        }

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