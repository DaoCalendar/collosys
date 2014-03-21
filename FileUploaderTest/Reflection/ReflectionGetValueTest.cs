using NUnit.Framework;
using FileUploader.Reflection;
using FileUploader.Reflection;

namespace FileUploader.Tests.Reflection
{
    [TestFixture]
    class ReflectionGetValueTest
    {
        private ReflectionPropertyNameTest.ReflectionTestHelperClass _helper;

        [SetUp]
        public void Init()
        {
             _helper = new ReflectionPropertyNameTest.ReflectionTestHelperClass();
        }

        [Test]
        public void Get_Property_Value()
        {
            _helper.Name = "algosys";
          var propertyValue = ReflectionHelper.GetValue<ReflectionPropertyNameTest.ReflectionTestHelperClass>(_helper, x => x.Name);
          Assert.AreEqual(propertyValue, _helper.Name);
        }

        [Test]
        public void Get_Field_Value()
        {
            _helper.Field = "algosys";
           var fieldValue = ReflectionHelper.GetValue<ReflectionPropertyNameTest.ReflectionTestHelperClass>(_helper, x => x.Field);
            Assert.AreEqual(fieldValue, _helper.Field);
        }

        [Test]
        public void Get_Nested_Property_Value()
        {
            _helper.ReflectionTestHelperClass1.Property = "Abhijeet";
            var propertyValue = ReflectionHelper.GetValue<ReflectionPropertyNameTest.ReflectionTestHelperClass>(_helper, x => x.ReflectionTestHelperClass1.ReflectionTestHelperClass2.Innerprop);
            Assert.AreEqual(propertyValue, _helper.ReflectionTestHelperClass1.ReflectionTestHelperClass2.Innerprop);
        }

        [Test]
        public void Get_Nested_InnerProp_Value()
        {
            _helper.ReflectionTestHelperClass1.ReflectionTestHelperClass2.Innerprop = "Abhijeet";
            var propertyValue = ReflectionHelper.GetValue<ReflectionPropertyNameTest.ReflectionTestHelperClass>(_helper, x => x.ReflectionTestHelperClass1.ReflectionTestHelperClass2.Innerprop);
            Assert.AreEqual(propertyValue, _helper.ReflectionTestHelperClass1.ReflectionTestHelperClass2.Innerprop);
        }

        [Test]
        public void Get_Nested_InnerField_Value()
        {
            _helper.ReflectionTestHelperClass1.ReflectionTestHelperClass2.InnerField = "algosys";
            var propertyValue = ReflectionHelper.GetValue<ReflectionPropertyNameTest.ReflectionTestHelperClass>(_helper, xyz => xyz.ReflectionTestHelperClass1.ReflectionTestHelperClass2.InnerField);
            Assert.AreEqual(propertyValue, _helper.ReflectionTestHelperClass1.ReflectionTestHelperClass2.InnerField);
        }
    }
}
