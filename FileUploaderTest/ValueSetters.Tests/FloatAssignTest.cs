using System;
using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    [TestFixture]
    class FloatAssignTest:AssignTypedTests<float>
    {
        public FloatAssignTest()
        {
           ValueSetter=new FloatSetter();
        }

       [TestCase("  123",123)]
       [TestCase("  (123)",-123)]
       [TestCase("  1,123,023", 1123023)]
       [TestCase(" -123.32", -123.32f)]
        public void Assigning_NumericString_WithSpaces_To_Float_Property(string value,float expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected , TypeTestHelper.Property);
        }

       [TestCase("  123", 123)]
       [TestCase("  (123)", -123)]
       [TestCase("  1,123,023", 1123023)]
       public void Assigning_NumericString_WithSpaces_To_Float_Field(string value, float expected)
       {
           SetFieldValue(value);
           Assert.AreEqual(expected, TypeTestHelper.Field);
       }

        [TestCase("algo")]
        [TestCase(null)]
        [TestCase("123?")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_NonNumbricString_To_Float_Property_ThrowsException(string value)
        {
            SetPropertyValue(value);
        }
        [TestCase("algo")]
        [TestCase(null)]
        [TestCase("123?")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_NonNumbricString_To_Float_Field_ThrowsException(string value)
        {
            SetFieldValue(value);
        }
     }
}
