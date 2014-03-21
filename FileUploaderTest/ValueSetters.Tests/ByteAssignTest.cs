using System;
using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    [TestFixture]
    class ByteAssignTest:AssignTypedTests<byte>
    {
        public ByteAssignTest()
        {
            ValueSetter=new ByteSetter();
            
        }

        [TestCase("123  ",123)]
        [TestCase("  123", 123)]
        [TestCase("0,123", 0123)]
        public void Assigning_NumericString_To_Byte_Property(string value,byte expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Property);
        }

        [TestCase("123  ", 123)]
        [TestCase("  123", 123)]
        [TestCase("0,123  ", 0123)]
        public void Assigning_NumericString_To_Byte_Field(string value, byte expected)
        {
            SetFieldValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Field);
        }

        [TestCase(null)]
        [TestCase("123?")]
        [TestCase("algo")]
        [TestCase("(123)")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_Null_Value_To_Byte_Property_ThrowsException(string value)
        {
            SetPropertyValue(value);
        }

        [TestCase(null)]
        [TestCase("123?")]
        [TestCase("algo")]
        [TestCase("(123)")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_Null_Value_To_Byte_Field_ThrowsException(string value)
        {
            SetFieldValue(value);
        }

    }
}
