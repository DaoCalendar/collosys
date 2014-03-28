using System;
using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{

    [TestFixture]
    class BooleanAssignTest : AssignTypedTests<bool>
    {
        public BooleanAssignTest()
        {
            ValueSetter = new BooleanSetter();
        }


        [TestCase("0", false)]
        [TestCase("1", true)]
        [TestCase("true", true)]
        [TestCase("false", false)]
        [TestCase("TRUE", true)]
        [TestCase("TRue", true)]
        [TestCase("TRuE", true)]
        [TestCase("FAlSE", false)]
        [TestCase("FAlse", false)]
        [TestCase("FAlSe", false)]
        [TestCase("True", true)]
        [TestCase("False", false)]
        [TestCase(null, false)]
        public void Assigning_NumbricString_To_Boolean_Property(string value, bool expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Property);
        }

        [TestCase("0", false)]
        [TestCase("1", true)]
        [TestCase("true", true)]
        [TestCase("false", false)]
        [TestCase("TRUE", true)]
        [TestCase("TRue", true)]
        [TestCase("TRuE", true)]
        [TestCase("FAlSE", false)]
        [TestCase("FAlse", false)]
        [TestCase("FAlSe", false)]
        [TestCase("True", true)]
        [TestCase("False", false)]
        [TestCase(null, false)]
        public void Assigning_NumbricString_Zero_To_Boolean_Field(string value, bool expected)
        {
            SetFieldValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Field);
        }

        [TestCase("123  ")]
        [TestCase("algosys")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_NumericString_WithSpaces_To_Boolean_Property_ThrowsException(string value)
        {
            SetPropertyValue(value);
        }

        [TestCase("123  ")]
        [TestCase("algosys")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_NumericString_WithSpaces_To_Boolean_Field_ThrowsException(string value)
        {
            SetFieldValue(value);
        }

    }
}
