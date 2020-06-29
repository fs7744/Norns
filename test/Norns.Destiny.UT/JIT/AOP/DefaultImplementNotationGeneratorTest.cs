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

        ValueTask<Task<T>> AddValueTask<T, V>(T v, V v1) where T : struct where V : class, IJitC;

        IEnumerable<T> AddValue1<T, V>(T v, ref V v1);

        IEnumerable<T> AddValue2<T, V>(T v, in V v1);

        IEnumerable<T> AddValue3<T, V>(T v, out V v1);
    }

    public struct A
    { }

    [Charon]
    public interface IJitD<out T> where T : class
    {
        T A();
    }

    [Charon]
    public interface IJitDIn<in T, V, R> where T : JitAopSourceGenerator
    {
        OnlyDefaultImplementNotationGenerator A();
    }

    public class DefaultImplementNotationGeneratorTest
    {
        [Fact]
        public async Task WhenSimpleInterfaceAndSomeMethods()
        {
            var types = JitTest.Generate(typeof(IJitC));
            Assert.Single(types);
            var t = types.Values.First();
            Assert.True(t.GetAttributes().Any(i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(IJitC)));
            var instance = Activator.CreateInstance(t.RealType) as IJitC;
            Assert.Equal(0, instance.AddOne(33));
            instance.AddVoid();
            await instance.AddTask(66);
            Assert.Equal(0, await instance.AddVTask(44));
            Assert.Equal(0, await instance.AddValueTask(11));
            Assert.Null(await instance.AddValueTask(this));
            Assert.Null(await instance.AddValueTask(new A(), instance));
            var c = instance;
            Assert.Null(instance.AddValue1(new A(), ref c));
            Assert.Null(instance.AddValue2(new A(), in c));
            Assert.Null(instance.AddValue3(new A(), out c));
        }

        [Fact]
        public void WhenOutGenericInterfaceSyncMethod()
        {
            var types = JitTest.Generate(typeof(IJitD<>));
            Assert.Single(types);
            var t = types.Values.First();
            Assert.True(t.GetAttributes().Any(i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(IJitD<>)));
            var instance = Activator.CreateInstance(t.RealType.MakeGenericType(this.GetType())) as IJitD<DefaultImplementNotationGeneratorTest>;
            Assert.Null(instance.A());
        }

        [Fact]
        public void WhenInGenericInterfaceSyncMethod()
        {
            var types = JitTest.Generate(typeof(IJitDIn<,,>));
            Assert.Single(types);
            var t = types.Values.First();
            Assert.True(t.GetAttributes().Any(i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(IJitDIn<,,>)));
            var instance = Activator.CreateInstance(t.RealType.MakeGenericType(typeof(JitAopSourceGenerator), typeof(int), typeof(int))) as IJitDIn<JitAopSourceGenerator, int, int>;
            Assert.Null(instance.A());
        }
    }
}