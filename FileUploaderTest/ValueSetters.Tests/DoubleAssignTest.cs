using System;
using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    [TestFixture]
    class DoubleAssignTest : AssignTypedTests<double>
    {
        public DoubleAssignTest()
        {
            ValueSetter = new DoubleSetter();
        }

        [TestCase("123  ", 123)]
        [TestCase("123  ", 123)]
        [TestCase("  123  ", 123)]
        [TestCase("(123)  ", -123)]
        [TestCase("01,123", 01123)]
        [TestCase("1,123,023", 1123023)]
        [TestCase("1,20,000", 120000)]
        public void Assigning_NumericString_WithSpaces_To_Double_Property(string value, double expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Property);
        }

        [TestCase("123  ", 123)]
        [TestCase("123  ", 123)]
        [TestCase("  123  ", 123)]
        [TestCase("(123)  ", -123)]
        [TestCase("01,123", 01123)]
        [TestCase("1,123,023", 1123023)]
        public void Assigning_NumericString_WithSpaces_To_Double_Field(string value, double expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Property);
        }

        [TestCase("algo")]
        [TestCase("123?")]
        [TestCase(null)]
        [ExpectedException(typeof(Exception))]
        public void Assigning_NonNumbricString_To_Double_Property_ThrowsException(string value)
        {
            SetPropertyValue(value);
        }

        [TestCase("algo")]
        [TestCase("123?")]
        [TestCase(null)]
        [ExpectedException(typeof(Exception))]
        public void Assigning_NonNumbricString_To_Double_Field_ThrowsException(string value)
        {
            SetFieldValue(value);
        }
    }

}

