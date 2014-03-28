using System;
using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    [TestFixture]
    public class UInt64AssignTest : AssignTypedTests<UInt64>
    {
        public UInt64AssignTest()
        {
            ValueSetter = new UInt64Setter();
        }

        [TestCase("123", 123u)]
        [TestCase("1,123", 1123u)]
        [TestCase("    1,123", 1123u)]
        [TestCase("123456789123", 123456789123u)]
        public void Assigning_NumbricString_To_UInt64_Property(string value, UInt64 expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Property);
        }

        [TestCase("123",123u)]
        [TestCase("1,123",1123u)]
        [TestCase("    1,123", 1123u)]
        [TestCase("123456789123", 123456789123u)]
        public void Assigning_NumbricString_To_UInt64_Field(string value,UInt64 expected)
        {
            SetFieldValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Field);
        }


        [TestCase(null)]
        [TestCase("Algosys")]
        [TestCase("-123")]
        [TestCase("(151)")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_String_To_UInt64_Property_ThrowsException(string value)
        {
            SetPropertyValue(value);
        }
        [TestCase(null)]
        [TestCase("Algosys")]
        [TestCase("-123")]
        [TestCase("(151)")]
        [TestCase(" ")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_String_To_UInt64_Field_ThrowsException(string value)
        {
            SetFieldValue(value);
        }
    }
}
