using System;
using Moq;
using NUnit.Framework;
using WcfServiceFaker;

namespace WcfServiceFakerFixture
{
    [TestFixture]
    public class ServerSideDynamicProxyFixture
    {
        public interface ITest
        {
            string TestFunctionOne();
            Object TestFunctionTwo(Object a, Object b);
        }

        [Test]
        public void Should_Be_Able_To_Call_AllMethods()
        {
            //arrange
            var mockDecoratedService = new Mock<ITest>();
            var arg1 = new Object();
            var arg2 = new Object();
            var expectedReturnObj = new Object();
            mockDecoratedService.Setup(m => m.TestFunctionTwo(arg1, arg2))
                .Returns(expectedReturnObj);
            IProxyConfig proxyConfigConfig;

            //act
            ITest test = (ITest)ServerSideDynamicProxy.NewInstance(mockDecoratedService.Object, out proxyConfigConfig);
            test.TestFunctionOne();
            var returnObj = test.TestFunctionTwo(arg1, arg2);

            //assert
            mockDecoratedService.Verify(a => a.TestFunctionOne());
            mockDecoratedService.Verify(a => a.TestFunctionTwo(arg1,arg2));
            Assert.AreSame(returnObj, expectedReturnObj);
		}


        [Test]
        public void Should_Allow_The_ServiceImplementation_To_Change_Inside_Proxy()
        {
            //arrange
            var mockDecoratedService = new Mock<ITest>();
            var mockDecoratedService2 = new Mock<ITest>();

            //act
            IProxyConfig proxyConfigConfig;
            ITest test = (ITest)ServerSideDynamicProxy.NewInstance(mockDecoratedService.Object, out proxyConfigConfig);
            test.TestFunctionOne();
            mockDecoratedService.Verify(a => a.TestFunctionOne());

            proxyConfigConfig.ServiceImplementation = mockDecoratedService2.Object;
            test.TestFunctionOne();
            mockDecoratedService2.Verify(a => a.TestFunctionOne());

        }
	}


}
