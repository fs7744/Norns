using Microsoft.Extensions.DependencyInjection;
using Norns.Verthandi.AOP;
using System.Threading.Tasks;
using Xunit;

namespace Norns.Destiny.UT.Verthandi.AOP
{
    public class ProxyNotationGeneratorTest
    {
        [Fact]
        public void WhenInheritInterface()
        {
            var instance = VerthandiTest.GenerateProxy<IJitD>();
            Assert.Equal(1, instance.GiveFive());
        }

        [Fact]
        public void WhenInheritClass()
        {
            var instance = VerthandiTest.GenerateProxy<IJitD>(action: s => s.AddTransient<IJitD, JitD>());
            Assert.Equal(8, instance.GiveFive());

            instance = VerthandiTest.GenerateProxy<IJitD>(action: s => s.AddSingleton<IJitD>(new JitD()));
            Assert.Equal(8, instance.GiveFive());

            instance = VerthandiTest.GenerateProxy<IJitD>(action: s => s.AddScoped<IJitD>(i => new JitD()));
            Assert.Equal(8, instance.GiveFive());

            instance = VerthandiTest.GenerateProxy<IJitD>(action: s => s.AddTransient<IJitD, JitD2>());
            Assert.Equal(100, instance.GiveFive());

            instance = VerthandiTest.GenerateProxy<IJitD>(action: s => s.AddSingleton<IJitD>(new JitD2(null)));
            Assert.Equal(100, instance.GiveFive());

            instance = VerthandiTest.GenerateProxy<IJitD>(action: s => s.AddScoped<IJitD>(i => new JitD2(i)));
            Assert.Equal(100, instance.GiveFive());
        }

        [Fact]
        public async Task WhenSimpleInterfaceAndSomeMethods()
        {
            var instance = VerthandiTest.GenerateProxy<IJitC>();
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
            Assert.Null(instance.AddValue2(new A(), in c));
            Assert.Null(instance.AddValue3(new A(), out c));
            Assert.Equal(8, instance.A());
        }

        [Fact]
        public void WhenOutGenericInterfaceSyncMethod()
        {
            var instance = VerthandiTest.GenerateProxy<IJitD<DataBase>>();
            Assert.Null(instance.A());
            instance = VerthandiTest.GenerateProxy<IJitD<DataBase>>(typeof(IJitD<>));
            Assert.Null(instance.A());
        }

        [Fact]
        public void WhenOutGenericInterfaceInClassBSyncMethod()
        {
            var instance = VerthandiTest.GenerateProxy<B.IJitDB<DataBase>>();
            Assert.Null(instance.A());

            var instance2 = VerthandiTest.GenerateProxy<B.A.IJitDA<Data>>();
            Assert.Null(instance2.A());
        }

        [Fact]
        public void WhenInGenericInterfaceSyncMethod()
        {
            var instance = VerthandiTest.GenerateProxy<IJitDIn<AopSourceGenerator, int, long?>>();
            Assert.Null(instance.A());
        }

        #region Abstract Class

        [Fact]
        public async Task WhenAbstractClassAndSomeMethods()
        {
            var instance = VerthandiTest.GenerateProxy<JitCClass>();
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
            Assert.Null(instance.AddValue2(new A(), in c));
            Assert.Null(instance.AddValue3(new A(), out c));
            Assert.Equal(3, instance.A());
            Assert.Equal(8, instance.B());
        }

        [Fact]
        public void WhenAbstractClassSyncMethod()
        {
            var instance = VerthandiTest.GenerateProxy<JitCClass<Data, int, long>>();
            var r = instance.A();
            Assert.Null(r.Item1);
            Assert.Equal(0, r.Item2);
            Assert.Equal(0, r.Item3);
        }

        [Fact]
        public void WhenNestedAbstractClassSyncMethod()
        {
            var instance = VerthandiTest.GenerateProxy<B.JitCClassB<Data, long, short>>();
            var r = instance.A();
            Assert.Null(r.Item1);
            Assert.Equal(0L, r.Item2);
            Assert.Equal(0, r.Item3);

            var instance1 = VerthandiTest.GenerateProxy<B.A.JitCClassA<DataBase, long, int>>();
            var r1 = instance1.A();
            Assert.Null(r1.Item1);
            Assert.Equal(0L, r1.Item2);
            Assert.Equal(0, r1.Item3);
        }

        #endregion Abstract Class
    }
}