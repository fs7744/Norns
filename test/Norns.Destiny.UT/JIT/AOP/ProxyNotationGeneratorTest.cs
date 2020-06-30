using System.Threading.Tasks;
using Xunit;

namespace Norns.Destiny.UT.JIT.AOP
{
    public class ProxyNotationGeneratorTest
    {
        [Fact]
        public void WhenInheritInterface()
        {
            var instance = JitTest.GenerateProxy<IJitD>();
            Assert.Equal(1, instance.GiveFive());
        }

        [Fact]
        public async Task WhenSimpleInterfaceAndSomeMethods()
        {
            var instance = JitTest.GenerateProxy<IJitC>();
            Assert.Equal(5, instance.AddOne(33));
            instance.AddVoid();
            await instance.AddTask(66);
            Assert.Equal(0, await instance.AddVTask(44));
            Assert.Equal(0, await instance.AddValueTask(11));
            Assert.Null(await instance.AddValueTask(this));
            Assert.Null(await instance.AddValueTask(new A(), instance));
            //Assert.Equal(-5, instance.PA);
            //instance.PD = 55;
            //Assert.Null(instance[3, ""]);
            var c = instance;
            //Assert.Null(instance.AddValue1(new A(), ref c));
            //Assert.Null(instance.AddValue2(new A(), in c));
            //Assert.Null(instance.AddValue3(new A(), out c));
            //Assert.Equal(8, instance.A());
        }

        #region Abstract Class

        //[Fact]
        //public async Task WhenAbstractClassAndSomeMethods()
        //{
        //    var instance = JitTest.GenerateProxy<JitCClass>();
        //    Assert.Equal(0, instance.AddOne(33));
        //    instance.AddVoid();
        //    await instance.AddTask(66);
        //    Assert.Equal(0, await instance.AddVTask(44));
        //    Assert.Equal(0, await instance.AddValueTask(11));
        //    Assert.Null(await instance.AddValueTask(this));
        //    Assert.Null(await instance.AddValueTask(new A(), instance));
        //    Assert.Equal(0, instance.PA);
        //    instance.PD = 55;
        //    Assert.Null(instance[3, ""]);
        //    var c = instance;
        //    Assert.Null(instance.AddValue1(new A(), ref c));
        //    Assert.Null(instance.AddValue2(new A(), in c));
        //    Assert.Null(instance.AddValue3(new A(), out c));
        //    Assert.Equal(3, instance.A());
        //    Assert.Equal(3, instance.B());
        //}

        #endregion Abstract Class
    }
}