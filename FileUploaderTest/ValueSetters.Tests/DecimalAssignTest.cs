using System;
using Microsoft.Win32.SafeHandles;
using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    [TestFixture]
    public class DecimalAssignTest:AssignTypedTests<decimal>
    {
        public DecimalAssignTest()
        {
            ValueSetter = new DecimalSetter();
        }
         
        [TestCase("12.321", 12.321)]
        [TestCase("(12.321564)", -12.321564)]
        [TestCase("1,123,215.64", 1123215.64)]
        [TestCase("(1,123,215.64)", -1123215.64)]
        [TestCase("$50.00",50.00)]
        [TestCase("-12.321", -12.321)]
        [TestCase("   -12.321   ", -12.321)]
        public void Assigning_NumbricString_With_Parentheses_To_Decimal_Property(string value,decimal expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected,TypeTestHelper.Property);
        }

        [TestCase("12.321", 12.321)]
        [TestCase("(12.321564)", -12.321564)]
        [TestCase("1,123,215.64", 1123215.64)]
        [TestCase("(1,123,215.64)", -1123215.64)]
        [TestCase("$50.00", 50.00)]
        [TestCase("12.321", 12.321)]
        [TestCase("-12.321", -12.321)]
        [TestCase("   -12.321   ", -12.321)]
        public void Assigning_NumbricString_With_Parentheses_To_Decimal_Field(string value, decimal expected)
        {
            SetFieldValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Field);
        }

        [TestCase(" ")]
        [TestCase(null)]
        [ExpectedException(typeof(Exception))]
        public void Assigning_BlankString_With_Space_To_Decimal_Property_ThrowsException(string value)
        {
            SetPropertyValue(value);
        }

        [TestCase(" ")]
        [TestCase(null)]
        [ExpectedException(typeof(Exception))]
        public void Assigning_BlankString_With_Space_To_Decimal_Field_ThrowsException(string value)
        {
            SetFieldValue(value);
        }

    }
}
