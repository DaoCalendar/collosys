using System;
using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    [TestFixture]
    class Int16AssignTest:AssignTypedTests<Int16>
    {
        public Int16AssignTest()
        {
            ValueSetter = new Int16Setter();
        }

        [TestCase("123", 123)]
        [TestCase("  123", 123)]
        [TestCase("  123", 123)]
        [TestCase("(123)", -123)]
        [TestCase("-123", -123)]
        [TestCase("1,123", 1123)]
        public void Assign_NumbricString_Value_To_Int16_Property(string value,Int16 expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected,TypeTestHelper.Property);
        }
        [TestCase("123", 123)]
        [TestCase("  123", 123)]
        [TestCase("  123", 123)]
        [TestCase("(123)", -123)]
        [TestCase("-123", -123)]
        [TestCase("1,123", 1123)]
        public void Assign_NumbricString_Value_To_Int16_Field(string value, Int16 expected)
        {
            SetFieldValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Field);
        }
        [TestCase("algo")]
        [TestCase("123.32")]
        [TestCase(null)]
        [ExpectedException(typeof(Exception))]
        public void Assigning_NonNumeric_String_To_Int16_Property_ThrowsException(string value)
        {
            SetPropertyValue(value);
        }

        [TestCase("algo")]
        [TestCase("123.32")]
        [TestCase(null)]
        [ExpectedException(typeof(Exception))]
        public void Assigning_NonNumeric_Or_FloatingString_To_Int16_Field_ThrowsException(string value)
        {
            SetFieldValue(value);
        }
    }
}
