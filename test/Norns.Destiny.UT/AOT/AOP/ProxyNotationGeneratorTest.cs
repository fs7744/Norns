using Norns.Destiny.AOT.Coder;
using Norns.Destiny.UT.JIT.AOP;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Norns.Destiny.UT.AOT.AOP
{
    public class ProxyNotationGeneratorTest
    {
        [Fact]
        public void WhenInheritInterface()
        {
            var code = @"
using Norns.Destiny.Attributes;
public interface ITest
    {
        long GiveFive();
    }

    [Charon]
    public interface IJitD : ITest
    {
    }";
            var typeName = "IJitD";
            var instance = AotTest.GenerateProxy(code, typeName);
            Assert.Equal(1, instance.GiveFive());
        }

        [Fact]
        public async Task WhenSimpleInterfaceAndSomeMethods()
        {
            var code = @"
using System.Collections.Generic;
using System.Threading.Tasks;
using Norns.Destiny.Attributes;
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
    }";
            var typeName = "IJitC";
            var instance = AotTest.GenerateProxy(code, typeName);
            Assert.Equal(5, instance.AddOne(33));
            instance.AddVoid();
            await instance.AddTask(66);
            Assert.Equal(0, await instance.AddVTask(44));
            Assert.Equal(0, await instance.AddValueTask(11));
            Assert.Null(await instance.AddValueTask(this));
            Assert.Null(await instance.AddValueTask(new A(), instance));
            Assert.Equal(-5, instance.PA);
            instance.PD = 55;
            Assert.Null(instance[3, ""]);
            var c = instance;
            Assert.Null(instance.AddValue1(new A(), ref c));
            //Assert.Null(instance.AddValue2(new A(), in c));
            Assert.Null(instance.AddValue3(new A(), out c));
            Assert.Equal(8, instance.A());
        }

        [Fact]
        public void WhenOutGenericInterfaceSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;
[Charon]
    public interface IJitD<out T> where T : class
    {
        T A();
    }";
            var typeName = "IJitD`1";
            var instance = AotTest.GenerateProxy(code, typeName, new Type[] { typeof(DataBase) });
            Assert.Null(instance.A());
            instance = JitTest.GenerateProxy<IJitD<DataBase>>(typeof(IJitD<>));
            Assert.Null(instance.A());
        }

        [Fact]
        public void WhenOutGenericInterfaceInClassBSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;

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
        }
    }";
            var typeName = "IJitDB`1";
            var instance = AotTest.GenerateProxy(code, typeName, new Type[] { typeof(DataBase) });
            Assert.Null(instance.A());

            typeName = "IJitDA`1";
            var instance2 = AotTest.GenerateProxy(code, typeName, new Type[] { typeof(DataBase) });
            Assert.Null(instance2.A());
        }

        [Fact]
        public void WhenInGenericInterfaceSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;
using Norns.Destiny.AOT.Coder;
using Norns.Destiny.AOT.AOP;

    [Charon]
    public interface IJitDIn<in T, V, R> where T : AotSourceGeneratorBase
    {
        AotAopSourceGenerator A();
    }
";
            var typeName = "IJitDIn`3";
            var instance = AotTest.GenerateProxy(code, typeName, new Type[] { typeof(AotSourceGeneratorBase), typeof(int), typeof(long?) });
            Assert.Null(instance.A());
        }

        #region Abstract Class

        [Fact]
        public async Task WhenAbstractClassAndSomeMethods()
        {
            var code = @"
using System.Collections.Generic;
using System.Threading.Tasks;
using Norns.Destiny.Attributes;
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
";
            var typeName = "JitCClass";
            var instance = AotTest.GenerateProxy(code, typeName);
            Assert.Equal(5, instance.AddOne(33));
            instance.AddVoid();
            await instance.AddTask(66);
            Assert.Equal(0, await instance.AddVTask(44));
            Assert.Equal(0, await instance.AddValueTask(11));
            Assert.Null(await instance.AddValueTask(this));
            Assert.Null(await instance.AddValueTask(new A(), instance));
            Assert.Equal(-5, instance.PA);
            instance.PD = 55;
            Assert.Null(instance[3, ""]);
            var c = instance;
            Assert.Null(instance.AddValue1(new A(), ref c));
            //Assert.Null(instance.AddValue2(new A(), in c));
            Assert.Null(instance.AddValue3(new A(), out c));
            Assert.Equal(3, instance.A());
            Assert.Equal(8, instance.B());
        }

        [Fact]
        public void WhenAbstractClassSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;
using Norns.Destiny.AOT.Coder;
using Norns.Destiny.AOT.AOP;

    [Charon]
    public abstract class JitCClass<T, V, R> where T : class
    {
        public abstract (T, V, R) A();
    }
";
            var typeName = "JitCClass`3";
            var instance = AotTest.GenerateProxy(code, typeName, new Type[] { typeof(Data), typeof(int), typeof(long) });
            var r = instance.A();
            Assert.Null(r.Item1);
            Assert.Equal(0, r.Item2);
            Assert.Equal(0, r.Item3);
        }

        [Fact]
        public void WhenNestedAbstractClassSyncMethod()
        {
            var code = @"
using Norns.Destiny.Attributes;
using Norns.Destiny.AOT.Coder;
using Norns.Destiny.AOT.AOP;

    public class B
    {
        public class A
        {
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
";
            var typeName = "JitCClassB`3";
            var instance = AotTest.GenerateProxy(code, typeName, new Type[] { typeof(Data), typeof(long), typeof(short) });
            var r = instance.A();
            Assert.Null(r.Item1);
            Assert.Equal(0L, r.Item2);
            Assert.Equal(0, r.Item3);

            typeName = "JitCClassA`3";
            var instance1 = AotTest.GenerateProxy(code, typeName, new Type[] { typeof(Data), typeof(long), typeof(int) });
            var r1 = instance1.A();
            Assert.Null(r1.Item1);
            Assert.Equal(0L, r1.Item2);
            Assert.Equal(0, r1.Item3);
        }

        #endregion Abstract Class
    }
}