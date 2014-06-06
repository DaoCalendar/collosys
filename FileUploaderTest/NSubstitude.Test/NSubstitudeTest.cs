using NSubstitute;
using NUnit.Framework;

namespace ReflectionExtension.Tests.NSubstitude.Test
{
    [TestFixture]
    public class NSubstitudeTest
    {

        [SetUp]
        public void Init()
        {
        }

        [Test]
        public void Test_Returns_from_Substitude()
        {
            //Arrange
            var objDemo = Substitute.For<NsubstitudeDemo>();
            objDemo.GetData().Returns("Algo");

            //Act
            string str = objDemo.GetData();

            //Assert
            Assert.AreEqual(objDemo.GetData(), "Algo");
        }

        [Test]
        public void NestedTest()
        {
            //Arrange
            var nestObj = Substitute.For<nested>();
            nestObj.obj.GetData().Returns("Abhijeet");

            //Arrange
            var obj = Substitute.For<INsubstitudeDemo>();
            var objnested = new nested();
            objnested.obj.GetData().Returns("abhijeet");

            //Act
            string str = nestObj.callGetData();

            //Assert
            Assert.AreEqual(str,"abhijeet");
        }
    }

}
