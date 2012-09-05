using System;
using NUnit.Framework;
using WcfServiceFakerFixture.DummyService;
using Moq;

namespace WcfServiceFakerFixture
{
    //[TestFixture]
    public class WcfServiceFakerFixture
    {

        [Test]
        public void Should_Be_Fast()
        {
            var startTime = DateTime.Now;
            const int iterations = 100;

            for (int i = 0; i < iterations; i++)
            {
                //arrange
                var mockServiceImplementation = new Mock<IDummyService>();
                mockServiceImplementation.Setup(m => m.DoWork("input")).Returns("expectedOutput");
                var client = new DummyServiceClient();
                //act
                WcfServiceFaker.WcfServiceFaker.SetWcfService<IDummyService>(mockServiceImplementation.Object);
                var output = client.DoWork("input");
                //assert
                Assert.AreEqual("expectedOutput", output);
            }

            var runTime = DateTime.Now.Subtract(startTime);

            Assert.IsTrue(runTime.TotalSeconds < 5, string.Format("Too slow. {0} service calls took {1} seconds", iterations, runTime.TotalSeconds));
        }

        [Test]
        public void Should_Call_The_Instance_That_Was_Set()
        {
            //arrange
            var mockServiceImplementation = new Mock<IDummyService>();
            mockServiceImplementation.Setup(m => m.DoWork("input")).Returns("expectedOutput");
            var client = new DummyServiceClient();
            //act
            WcfServiceFaker.WcfServiceFaker.SetWcfService<IDummyService>(mockServiceImplementation.Object);
            var output = client.DoWork("input");
            //assert
            Assert.AreEqual("expectedOutput",output);
        }

        [Test]
        public void Should_Support_ManyServiceImplementationInstances()
        {
            //arrange
            var mockServiceImplementation1 = new Mock<IDummyService>();
            mockServiceImplementation1.Setup(m => m.DoWork("")).Returns("expectedOutputInstance1");

            var mockServiceImplementation2 = new Mock<IDummyService>();
            mockServiceImplementation2.Setup(m => m.DoWork("")).Returns("expectedOutputInstance2");

            var client = new DummyServiceClient();

            //act
            WcfServiceFaker.WcfServiceFaker.SetWcfService<IDummyService>(mockServiceImplementation1.Object);
            var outputInstance1 = client.DoWork("");

            WcfServiceFaker.WcfServiceFaker.SetWcfService<IDummyService>(mockServiceImplementation2.Object);
            var outputInstance2 = client.DoWork("");

            //assert
            Assert.AreEqual("expectedOutputInstance1", outputInstance1);
            Assert.AreEqual("expectedOutputInstance2", outputInstance2);
        }

        [Test]
        public void Should_Support_ManyServiceTypes()
        {
            //arrange
            var mockServiceType1 = new Mock<IDummyService>();
            mockServiceType1.Setup(m => m.DoWork("")).Returns("expectedOutputType1");
            var client1 = new DummyServiceClient();

            var mockServiceType2 = new Mock<DummyService2.IDummyService>();
            mockServiceType2.Setup(m => m.DoWork("")).Returns("expectedOutputType2");
            var client2 = new DummyService2.DummyServiceClient();


            //act
            WcfServiceFaker.WcfServiceFaker.SetWcfService<IDummyService>(mockServiceType1.Object);
            var outputInstance1 = client1.DoWork("");

            WcfServiceFaker.WcfServiceFaker.SetWcfService<DummyService2.IDummyService>(mockServiceType2.Object);
            var outputInstance2 = client2.DoWork("");

            //assert
            Assert.AreEqual("expectedOutputType1", outputInstance1);
            Assert.AreEqual("expectedOutputType2", outputInstance2);
        }

    }
}
