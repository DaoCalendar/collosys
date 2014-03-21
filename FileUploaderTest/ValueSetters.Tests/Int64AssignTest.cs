using System;
using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    [TestFixture]
    class Int64AssignTest : AssignTypedTests<Int64>
    {
        public Int64AssignTest()
        {
            ValueSetter = new Int64Setters();
        }


        [TestCase("123   ", 123)]
        [TestCase("(123)   ", -123)]
        [TestCase("-123", -123)]
        [TestCase("10,123,321", 10123321)]
        [TestCase("(10,123,321)", -10123321)]
        [TestCase("1,20,000",120000)]
        public void Assigning_NumericString_To_Int64_Property(string value, Int64 expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Property);
        }

        [TestCase("123   ", 123)]
        [TestCase("(123)   ", -123)]
        [TestCase("-123", -123)]
        [TestCase("10,123,321", 10123321)]
        [TestCase("(10,123,321)", -10123321)]
        public void Assigning_NumericString_To_Int64_Field(string value, Int64 expected)
        {
            SetFieldValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Field);
        }

        [TestCase("123.32")]
        [TestCase(null)]
        [TestCase("1,123,023.20")]
        [TestCase("Algo")]
        [TestCase("123?")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_Floating_Or_NonNumbricString_With_Thousands_To_Int64_Property_ThrowsException(string value)
        {
            SetPropertyValue(value);
        }

        [TestCase("123.32")]
        [TestCase(null)]
        [TestCase("1,123,023.20")]
        [TestCase("Algo")]
        [TestCase("123?")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_Floating_Or_NonNumbricString_With_Thousands_To_Int64_Field_ThrowsException(string value)
        {
            SetFieldValue(value);
        }

    }
}
