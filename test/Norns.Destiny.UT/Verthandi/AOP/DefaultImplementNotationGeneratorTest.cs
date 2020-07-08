using Norns.Destiny.Attributes;
using Norns.Verthandi.AOP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Norns.Destiny.UT.Verthandi.AOP
{
    public interface ITest
    {
        long GiveFive();
    }

    [Charon]
    public interface IJitD : ITest
    {
    }

    public class JitD : IJitD
    {
        public long GiveFive()
        {
            return 7;
        }
    }

    public class JitD2 : IJitD
    {
        private readonly IServiceProvider provider;

        public JitD2(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public long GiveFive()
        {
            return 99;
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

        public int A() => 3;

        int PA { get; set; }

        int PD { set; }

        string this[int v, string s] { get; set; }
    }

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

        public abstract string this[int v, string s] { get; set; }
    }

    public struct A
    { }

    [Charon]
    public interface IJitD<out T> where T : class
    {
        T A();
    }

    [Charon]
    public abstract class JitCClass<T, V, R> where T : class
    {
        public abstract (T, V, R) A();
    }

    [Charon]
    public interface IJitDIn<in T, V, R> where T : AopSourceGenerator
    {
        OnlyDefaultImplementNotationGenerator A();
    }

    public class B
    {
        [Charon]
        public interface IJitDB<out T> where T : class
        {
            T A();
        }

        public class A
        {
            [Charon]
            public interface IJitDA<out T> where T : class
            {
                T A();
            }

            [Charon]
            public abstract class JitCClassA<T, V, R> where T : class
            {
                public abstract (T, V, R) A();
            }
        }

        [Charon]
        public abstract class JitCClassB<T, V, R> where T : class
        {
            public abstract (T, V, R) A();
        }
    }

    public class DataBase
    {
    }

    public class Data : DataBase
    { }

    public class DefaultImplementNotationGeneratorTest
    {
        #region Interface

        [Fact]
        public void WhenInheritInterface()
        {
            var types = VerthandiTest.Generate(typeof(IJitD));
            Assert.Single(types);
            var t = types.Values.First();
            Assert.Contains(t.Attributes, i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(IJitD));
            var instance = Activator.CreateInstance(t.RealType) as IJitD;
            Assert.Equal(0, instance.GiveFive());
        }

        [Fact]
        public async Task WhenSimpleInterfaceAndSomeMethods()
        {
            var types = VerthandiTest.Generate(typeof(IJitC));
            Assert.Single(types);
            var t = types.Values.First();
            Assert.Contains(t.Attributes, i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(IJitC));
            var instance = Activator.CreateInstance(t.RealType) as IJitC;
            Assert.Equal(0, instance.AddOne(33));
            instance.AddVoid();
            await instance.AddTask(66);
            Assert.Equal(0, await instance.AddVTask(44));
            Assert.Equal(0, await instance.AddValueTask(11));
            Assert.Null(await instance.AddValueTask(this));
            Assert.Null(await instance.AddValueTask(new A(), instance));
            Assert.Equal(0, instance.PA);
            instance.PD = 55;
            Assert.Null(instance[3, ""]);
            var c = instance;
            Assert.Null(instance.AddValue1(new A(), ref c));
            Assert.Null(instance.AddValue2(new A(), in c));
            Assert.Null(instance.AddValue3(new A(), out c));
            Assert.Equal(3, instance.A());
        }

        [Fact]
        public void WhenOutGenericInterfaceSyncMethod()
        {
            var types = VerthandiTest.Generate(typeof(IJitD<>));
            Assert.Single(types);
            var t = types.Values.First();
            Assert.Contains(t.Attributes, i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(IJitD<>));
            var instance = Activator.CreateInstance(t.RealType.MakeGenericType(this.GetType())) as IJitD<DefaultImplementNotationGeneratorTest>;
            Assert.Null(instance.A());
        }

        [Fact]
        public void WhenOutGenericInterfaceInClassBSyncMethod()
        {
            var types = VerthandiTest.Generate(typeof(B.IJitDB<>));
            Assert.Single(types);
            var t = types.Values.First();
            Assert.Contains(t.Attributes, i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(B.IJitDB<>));
            var instance = Activator.CreateInstance(t.RealType.MakeGenericType(this.GetType())) as B.IJitDB<DefaultImplementNotationGeneratorTest>;
            Assert.Null(instance.A());

            types = VerthandiTest.Generate(typeof(B.A.IJitDA<>));
            Assert.Single(types);
            t = types.Values.First();
            Assert.Contains(t.Attributes, i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(B.A.IJitDA<>));
            var instance2 = Activator.CreateInstance(t.RealType.MakeGenericType(this.GetType())) as B.A.IJitDA<DefaultImplementNotationGeneratorTest>;
            Assert.Null(instance2.A());
        }

        [Fact]
        public void WhenInGenericInterfaceSyncMethod()
        {
            var types = VerthandiTest.Generate(typeof(IJitDIn<,,>));
            Assert.Single(types);
            var t = types.Values.First();
            Assert.Contains(t.Attributes, i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(IJitDIn<,,>));
            var instance = Activator.CreateInstance(t.RealType.MakeGenericType(typeof(AopSourceGenerator), typeof(int), typeof(int))) as IJitDIn<AopSourceGenerator, int, int>;
            Assert.Null(instance.A());
        }

        #endregion Interface

        #region Abstract Class

        [Fact]
        public async Task WhenAbstractClassAndSomeMethods()
        {
            var types = VerthandiTest.Generate(typeof(JitCClass));
            Assert.Single(types);
            var t = types.Values.First();
            Assert.Contains(t.Attributes, i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(JitCClass));
            var instance = Activator.CreateInstance(t.RealType) as JitCClass;
            Assert.Equal(0, instance.AddOne(33));
            instance.AddVoid();
            await instance.AddTask(66);
            Assert.Equal(0, await instance.AddVTask(44));
            Assert.Equal(0, await instance.AddValueTask(11));
            Assert.Null(await instance.AddValueTask(this));
            Assert.Null(await instance.AddValueTask(new A(), instance));
            Assert.Equal(0, instance.PA);
            instance.PD = 55;
            Assert.Null(instance[3, ""]);
            var c = instance;
            Assert.Null(instance.AddValue1(new A(), ref c));
            Assert.Null(instance.AddValue2(new A(), in c));
            Assert.Null(instance.AddValue3(new A(), out c));
            Assert.Equal(3, instance.A());
            Assert.Equal(3, instance.B());
        }

        [Fact]
        public void WhenAbstractClassSyncMethod()
        {
            var types = VerthandiTest.Generate(typeof(JitCClass<,,>));
            Assert.Single(types);
            var t = types.Values.First();
            Assert.Contains(t.Attributes, i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(JitCClass<,,>));
            var instance = Activator.CreateInstance(t.RealType.MakeGenericType(this.GetType(), typeof(long), typeof(int))) as JitCClass<DefaultImplementNotationGeneratorTest, long, int>;
            var r = instance.A();
            Assert.Null(r.Item1);
            Assert.Equal(0L, r.Item2);
            Assert.Equal(0, r.Item3);
        }

        [Fact]
        public void WhenNestedAbstractClassSyncMethod()
        {
            var types = VerthandiTest.Generate(typeof(B.JitCClassB<,,>));
            Assert.Single(types);
            var t = types.Values.First();
            Assert.Contains(t.Attributes, i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(B.JitCClassB<,,>));
            var instance = Activator.CreateInstance(t.RealType.MakeGenericType(this.GetType(), typeof(long), typeof(int))) as B.JitCClassB<DefaultImplementNotationGeneratorTest, long, int>;
            var r = instance.A();
            Assert.Null(r.Item1);
            Assert.Equal(0L, r.Item2);
            Assert.Equal(0, r.Item3);

            types = VerthandiTest.Generate(typeof(B.A.JitCClassA<,,>));
            Assert.Single(types);
            t = types.Values.First();
            Assert.Contains(t.Attributes, i => i.AttributeType.FullName == typeof(DefaultImplementAttribute).FullName && i.ConstructorArguments.First().Value == typeof(B.A.JitCClassA<,,>));
            var instance1 = Activator.CreateInstance(t.RealType.MakeGenericType(this.GetType(), typeof(long), typeof(int))) as B.A.JitCClassA<DefaultImplementNotationGeneratorTest, long, int>;
            r = instance1.A();
            Assert.Null(r.Item1);
            Assert.Equal(0L, r.Item2);
            Assert.Equal(0, r.Item3);
        }

        #endregion Abstract Class
    }
}
