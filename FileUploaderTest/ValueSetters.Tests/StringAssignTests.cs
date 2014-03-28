using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    [TestFixture]
    public class StringAssignTests : AssignTypedTests<string>
    {
        public StringAssignTests()
        {
            ValueSetter = new StringSetter();
        }

       [TestCase("","")]
       [TestCase(null,null)]
       [TestCase("123.321asd","123.321asd")]
       [TestCase("!@$#","!@$#")]
       public void Assigning_String_To_String_Property(string value, string expected)
        {
            SetPropertyValue(value);
            Assert.AreEqual(expected, TypeTestHelper.Property);
        }

       [TestCase("", "")]
       [TestCase(null, null)]
       [TestCase("123.321asd", "123.321asd")]
       [TestCase("!@$#", "!@$#")]
       public void Assigning_String_To_String_Field(string value, string expected)
       {
           SetFieldValue(value);
           Assert.AreEqual(expected, TypeTestHelper.Field);
       }

    }
}
