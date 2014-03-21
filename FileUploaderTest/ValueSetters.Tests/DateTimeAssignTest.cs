using System;
using NUnit.Framework;
using FileUploader.ValueSetters;

namespace FileUploader.Tests.ValueSetters.Tests
{
    [TestFixture]
    class DateTimeAssignTest : AssignTypedTests<DateTime>
    {
        public DateTimeAssignTest()
        {
            ValueSetter = new DateTimeSetter();
        }

        [TestCase("06-12-2002 01:12:02", "12-06-2002 01:12:02", "dd-MM-yyyy HH:mm:ss")] //Dash between dd-mm-yyyy with format
        [TestCase("6/12/2002 12:11:22 AM ", "12,6,2002 12:11:22 AM","")]                //slash between dd/mm/yyyy With time
        [TestCase("06-12-2002", "2002-12-06", "dd-MM-yyyy")]                            //only date with format
        [TestCase("6,12,2002 ", "12,06,2002 ","")]                                      //Date with comma        
        [TestCase("18-Nov-03 11:25 ", "11/18/2003 11:25", "")]                          //Date with month name like 18-Nov-03
        [TestCase("831.01289631332 ", "4/10/1902 12:18:34.241 AM","")]                  //OADate with time
        [TestCase("011202 ", "1:12:02 AM", "HHmmss")]                                   //Time Without separator
        [TestCase("12:11:22 AM", "12:11:22 AM", "")]                                    //Time with : separator
        [TestCase("12,11,22 AM", "12,11,22 AM", "")]                                    //Time with Comma separator
        [TestCase("6-12-2002 12:11:22 AM ", "12-6-2002 12:11:22 AM ","")]               //date and time with dash separator
        [TestCase("6.12.2002 12:11:22", "12-6-2002 12:11:22 ","")]                      //Date time with Dot separator
        public void Assigning_DateTimeString_WithFormat_To_DateTime_Property(string value, string expected, string format = "")
        {
            SetPropertyValue(value, format);
            Assert.AreEqual(DateTime.Parse(expected), TypeTestHelper.Property);
        }

        [TestCase("06-12-2002 01:12:02", "12-06-2002 01:12:02", "dd-MM-yyyy HH:mm:ss")]  //Dash between dd-mm-yyyy with format
        [TestCase("6/12/2002 12:11:22 AM ", "12-6-2002 12:11:22 AM", "")]                //slash between dd/mm/yyyy With time
        [TestCase("06-12-2002", "12/06/2002", "dd-MM-yyyy")]                             //only date with format
        [TestCase("6,12,2002 12:11:22 ", "12,6,2002 12:11:22", "")]                      //Date with comma 
        [TestCase("18-Nov-03 11:25 ", "11/18/2003 11:25", "")]                           //Date with month name like 18-Nov-03
        [TestCase("831.01289631332 ", "4/10/1902 12:18:34.241 AM", "")]                  //OADate with time
        [TestCase("011202 ", "1:12:02 AM", "HHmmss")]                                    //Time Without separator
        [TestCase("12:11:22 AM", "12:11:22 AM", "")]                                     //Time with : separator
        [TestCase("12,11,22 AM", "12,11,22 AM", "")]                                     //Time with Comma separator
        [TestCase("6-12-2002 12:11:22 AM ", "12-6-2002 12:11:22 AM ", "")]               //date and time with dash separator
        [TestCase("6.12.2002 12:11:22", "12-6-2002 12:11:22 ", "")]                      //Date time with Dot separator
        public void Assigning_DateTimeString_WithFormat_To_DateTime_Field(string value, string expected, string format = "")
        {
           SetFieldValue(value, format);
            Assert.AreEqual(DateTime.Parse(expected), TypeTestHelper.Field);
        }

        [TestCase("algosys")]
        [TestCase(null)]
        
        [ExpectedException(typeof(Exception))]
        public void Assigning_NonNumbricString_To_DateTime_Property_ThrowsException(string value)
        {
            SetPropertyValue(value);
        }

        [TestCase("algosys")]
        [TestCase(null)]
        [ExpectedException(typeof(Exception))]
        public void Assigning_NonNumbricString_To_DateTime_Field_ThrowsException(string value)
        {
            SetFieldValue(value);
        }

        //[Test]
        //public void Assigning_DateTimeString_WithFormat_To_DateTime_Field()
        //{
        //    SetPropertyValue("06-12-2002 01:12:02", "dd-MM-yyyy HH:mm:ss");
        //    Assert.AreEqual(DateTime.Parse("12-06-2002 01:12:02"), TypeTestHelper.Property);
        //}
        //[Test]
        //public void Assigning_OnlyTimeString_WithFormat_To_DateTime_Property()
        //{
        //    SetPropertyValue("011202", "HHmmss");
        //    Assert.AreEqual(DateTime.Parse("1:12:02 AM"), TypeTestHelper.Property);
        //}
        //[Test]
        //public void Assigning_OnlyTimeString_WithFormate_To_DateTime_Field()
        //{
        //    SetFieldValue("011202", "HHmmss");
        //    Assert.AreEqual(DateTime.Parse("1:12:02 AM"), TypeTestHelper.Field);
        //}

        //[Test]
        //public void Assigning_OnlyDateString_WithFormat_To_DateTime_Property()
        //{
        //    SetPropertyValue("06-12-2002", "dd-MM-yyyy");
        //    Assert.AreEqual(DateTime.Parse("2002-12-06"), TypeTestHelper.Property);
        //}
        //[Test]
        //public void Assigning_DateString_WithFormat_To_DateTime_Field()
        //{
        //    SetFieldValue("06-12-2002", "dd-MM-yyyy");
        //    Assert.AreEqual(DateTime.Parse("2002-12-06"), TypeTestHelper.Field);
        //}
        //[Test]
        //public void Assigning_OnlyTimeString_WithColon_To_DateTime_Property()
        //{
        //    SetPropertyValue("12:11:22 AM");
        //    Assert.AreEqual(DateTime.Parse("12:11:22 AM"), TypeTestHelper.Property);
        //}
        //[Test]
        //public void Assigning_OnlyTimeString_WithColon_To_DateTime_Field()
        //{
        //    SetFieldValue("12:11:22 AM");
        //    Assert.AreEqual(DateTime.Parse("12:11:22 AM"), TypeTestHelper.Field);
        //}
        //[Test]
        //public void Assigning_OnlyTimeString_WithComma_To_DateTime_Property()
        //{
        //    SetPropertyValue("12,11,22 AM");
        //    Assert.AreEqual(DateTime.Parse("12,11,22 AM"), TypeTestHelper.Property);
        //}
        //[Test]
        //public void Assigning_OnlyTimeString_WithComma_To_DateTime_Field()
        //{
        //    SetFieldValue("12,11,22 AM");
        //    Assert.AreEqual(DateTime.Parse("12,11,22 AM"), TypeTestHelper.Field);
        //}
        //[Test]
        //public void Assigning_DateString_Withdash_Symbole_To_DateTime_Property()
        //{
        //    SetPropertyValue("6-12-2002 12:11:22 AM");
        //    Assert.AreEqual(DateTime.Parse("12-6-2002 12:11:22 AM"), TypeTestHelper.Property);
        //}

        //[Test]
        //public void Assigning_DateString_dash_Betweenddmmyyyy_To_DateTime_Field()
        //{
        //    SetFieldValue("6-12-2002 12:11:22 AM ");
        //    Assert.AreEqual(DateTime.Parse("12-6-2002 12:11:22 AM"), TypeTestHelper.Field);
        //}
        //[Test]
        //public void Assigning_DateString_Slash_Betweenddmmyyyy_To_DateTime_Property()
        //{
        //    SetPropertyValue("6/12/2002 12:11:22 AM ");
        //    Assert.AreEqual(DateTime.Parse("12,6,2002 12:11:22 AM"), TypeTestHelper.Property);
        //}
        //[Test]
        //public void Assigning_DateString_Slash_Betweenddmmyyyy_To_DateTime_Field()
        //{
        //    SetFieldValue("6/12/2002 12:11:22 AM ");
        //    Assert.AreEqual(DateTime.Parse("12,6,2002 12:11:22 AM"), TypeTestHelper.Field);
        //}
        //[Test]
        //public void Assigning_DateString_Comma_Betweenddmmyyyy_To_DateTime_Property()
        //{
        //    SetPropertyValue("6,12,2002 12:11:22 AM ");
        //    Assert.AreEqual(DateTime.Parse("12,6,2002 12:11:22 AM"), TypeTestHelper.Property);
        //}

        //[Test]
        //public void Assigning_DateString_Comma_Betweenddmmyyyy_To_DateTime_Field()
        //{
        //    SetFieldValue("6,12,2002 12:11:22 AM ");
        //    Assert.AreEqual(DateTime.Parse("12,6,2002 12:11:22 AM"), TypeTestHelper.Field);
        //}

        //[Test]
        //public void Assigning_DateString_WithMonth_Name_To_DateTime_Field()
        //{
        //    SetFieldValue(" 18-Nov-03 11:25");
        //    Assert.AreEqual(DateTime.Parse("11/18/2003 11:25"), TypeTestHelper.Field);
        //}

        //[Test]
        //public void Assigning_DateString_WithMonth_Name_To_DateTime_Property()
        //{
        //    SetPropertyValue(" 18-Nov-03 11:25");
        //    Assert.AreEqual(DateTime.Parse("11/18/2003 11:25"), TypeTestHelper.Property);
        //}
        //[Test]
        //public void Assigning_OADateString_To_DateTime_Property()
        //{
        //    SetPropertyValue("831.01289631332");
        //    Assert.AreEqual(DateTime.Parse("4/10/1902 12:18:34.241 AM"), TypeTestHelper.Property);
        //}
        //[Test]
        //public void Assigning_OADateString_To_DateTime_Field()
        //{
        //    SetFieldValue("831.01289631332");
        //    Assert.AreEqual(DateTime.Parse("4/10/1902 12:18:34.241 AM"), TypeTestHelper.Field);
        //}
    }
}
