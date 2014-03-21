using System;
using Microsoft.Win32.SafeHandles;
using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    [TestFixture]
    class CharAssignTest:AssignTypedTests<char>
    {
        public CharAssignTest()
        {
            ValueSetter=new CharSetter();
        }

        [TestCase("1",'1')]
        [TestCase("$",'$')]
        [TestCase("A",'A')]
        [TestCase("a", 'a')]
        public void Assigning_NumbricString_Value_To_Char_Property(string value,char expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Property);
        }

        [TestCase("1", '1')]
        [TestCase("$", '$')]
        [TestCase("A", 'A')]
        [TestCase("a", 'a')]
        public void Assigning_NumbricString_Value_To_Char_Field(string value, char expected)
        {
            SetFieldValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Field);
        }

        [TestCase(null)]
        [TestCase("algosys")]
        [TestCase(" ")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_Null_Value_To_Char_Property_ThrowsException(string value)
        {
            SetPropertyValue(value);
        }

        [TestCase(null)]
        [TestCase("algosys")]
        [TestCase(" ")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_Null_Value_To_Char_Field_ThrowsException(string value)
        {
            SetFieldValue(value);
        }

    }
}
