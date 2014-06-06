using System;
using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    [TestFixture]
    public class UInt32AssignTest : AssignTypedTests<UInt32>
    {
        public UInt32AssignTest()
        {
            ValueSetter = new UInt32Setter();
        }

        [TestCase("123", 123u)]
        [TestCase("2,123", 2123u)]
        [TestCase("  2,123", 2123u)]
        public void Assigning_NumbricString_To_UInt32_Property(string value, UInt32 expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Property);
        }

        [TestCase("123", 123u)]
        [TestCase("2,123", 2123u)]
        [TestCase("  2,123", 2123u)]
        public void Assigning_NumbricString_To_UInt32_Field(string value, UInt32 expected)
        {
            SetFieldValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Field);
        }

        [TestCase(null)]
        [TestCase("-123")]
        [TestCase("  ")]
        [TestCase("123456789123")]
        [TestCase("(123)")]
        [TestCase("Algosys")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_String_To_UInt32_property_ThrowsException(string value)
        {
            SetPropertyValue(value);
        }

        [TestCase(null)]
        [TestCase("-123")]
        [TestCase("  ")]
        [TestCase("123456789123")]
        [TestCase("(123)")]
        [TestCase("Algosys")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_String_To_UInt32_Field_ThrowsException(string value)
        {
            SetFieldValue(value);
        }
      
    }
}
