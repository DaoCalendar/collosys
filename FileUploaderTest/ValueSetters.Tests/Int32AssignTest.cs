using System;
using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    [TestFixture]
    public class Int32AssignTest : AssignTypedTests<Int32>
    {
        public Int32AssignTest()
        {
            ValueSetter = new Int32Setter();
        }

        [TestCase("123", 123)]
        [TestCase("  123", 123)]
        [TestCase("  123", 123)]
        [TestCase("(123)", -123)]
        [TestCase("-123", -123)]
        [TestCase("1,123", 1123)]
        public void Assigning_NumericString_WithSpaces_To_Int32_Property(string value, Int32 expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Property);
        }

        [TestCase("123", 123)]
        [TestCase("  123", 123)]
        [TestCase("  123", 123)]
        [TestCase("(123)", -123)]
        [TestCase("-123", -123)]
        [TestCase("1,123", 1123)]
        public void Assigning_NumericString_WithSpaces_To_Int32_Field(string value, Int32 expected)
        {
            SetFieldValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Field);
        }

        [TestCase("algo")]
        [TestCase("123.32")]
        [TestCase(null)]
        [ExpectedException(typeof(Exception))]
        public void Assigning_NonNumeric_String_To_Int32_Property_ThrowsException(string value)
        {
            SetPropertyValue(value);
        }

        [TestCase("algo")]
        [TestCase("123.32")]
        [TestCase(null)]
        [ExpectedException(typeof(Exception))]
        public void Assigning_NonNumeric_Or_FloatingString_To_Int32_Field_ThrowsException(string value)
        {
            SetFieldValue(value);
        }






    }
}
