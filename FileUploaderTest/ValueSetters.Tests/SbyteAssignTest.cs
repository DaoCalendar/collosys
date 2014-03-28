using System;
using System.Reflection;
using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    [TestFixture]
    class SbyteAssignTest:AssignTypedTests<sbyte>
    {
        public SbyteAssignTest()
        {
            ValueSetter=new  SbyteSetter();
        }

       [TestCase("123  ",123)]
       [TestCase("(123)  ",-123)]
       [TestCase("-123  ",-123)]
       [TestCase("0,123  ",0123)]
        public void Assigning_NumericString_WithSpaces_To_Sbyte_Property(string value,sbyte expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Property);
        }

       [TestCase("123  ", 123)]
       [TestCase("(123)  ", -123)]
       [TestCase("-123  ", -123)]
       [TestCase("0,123  ", 0123)]
       public void Assigning_NumericString_WithSpaces_To_Sbyte_Field(string value, sbyte expected)
       {
           SetFieldValue(value);
           Assert.AreEqual(expected, TypeTestHelper.Field);
       }

        [TestCase("123?")]
        [TestCase(null)]
        [TestCase("algo")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_Null_Value_To_Sbyte_Property_ThrowsException(string value)
        {
            SetPropertyValue(value);
        }

        [TestCase("123?")]
        [TestCase(null)]
        [TestCase("algo")]
        [ExpectedException(typeof(Exception))]
        public void Assigning_Null_Value_To_Sbyte_Field_throwsException(string value)
        {
            SetFieldValue(value);
        }

    }
}
