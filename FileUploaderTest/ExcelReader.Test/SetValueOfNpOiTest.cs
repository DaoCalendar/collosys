using System.Collections.Generic;
using ColloSys.FileUploaderService.ExcelReader;
using ColloSys.UserInterface.Areas.Developer.Models.Excel2Db;
using FileUploader.ExcelReader;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;

namespace ReflectionExtension.Tests.ExcelReader.Test
{
    [TestFixture]
    class SetValueOfNpOiTest
    {
        private ConvertExcelToList<ExcelReaderHelper> _data;
        private IExcelReader _excel;
        private List<MappingInfo> _list;
        private IList<ExcelReaderHelper> _objList;
        
        [SetUp]
        public void Init()
        {
            var fileInfo = ResourceReader.GetEmbeddedResourceAsFileStream("ExampleData.xls");
            _excel=new NpOiExcelReader(fileInfo);
            _data=new ConvertExcelToList<ExcelReaderHelper>(_excel);
            _list = ExcelReaderHelper.GetMappingInfo();
           _objList= _data.GetList(_list);
        }
        [Test]
        public void Test_SetValue_To_List_Column_string()
        {
            var val = ExcelReaderHelper.ReadList(_objList, "Property1", 0);
            var val2 = ExcelReaderHelper.ReadList(_objList, "Property1", 1);
            var val3 = ExcelReaderHelper.ReadList(_objList, "Property1", 2);
            var val4 = ExcelReaderHelper.ReadList(_objList, "Property1", 3);
            var val5 = ExcelReaderHelper.ReadList(_objList, "Property1", 4);
            var val6 = ExcelReaderHelper.ReadList(_objList, "Property1", 5);

            Assert.AreEqual(val, "Skydiving");
            Assert.AreEqual(val2, "Elevator");
            Assert.AreEqual(val3, "algo");
            Assert.AreEqual(val4, "negativeNumbers");
            Assert.AreEqual(val5, "Thousands");
            Assert.AreEqual(val6, "non Numbric string");
        }

        [Test]
        public void Test_SetValue_To_List_Column_Int32()
        {
            var val = ExcelReaderHelper.ReadList(_objList, "Property2", 0);
            var val2 = ExcelReaderHelper.ReadList(_objList, "Property2", 1);
            var val3 = ExcelReaderHelper.ReadList(_objList, "Property2", 2);
            var val4 = ExcelReaderHelper.ReadList(_objList, "Property2", 3);
            var val5 = ExcelReaderHelper.ReadList(_objList, "Property2", 4);
            var val6 = ExcelReaderHelper.ReadList(_objList, "Property2", 5);

            Assert.AreEqual(val, "710");
            Assert.AreEqual(val2, "575");
            Assert.AreEqual(val3, "10000");
            Assert.AreEqual(val4, "-3214");
            Assert.AreEqual(val5, "123321");
            Assert.AreEqual(val6, "123");
        }
        [Test]
        public void Test_SetValue_To_List_Column_DateTime()
        {
            var val = ExcelReaderHelper.ReadList(_objList, "Property3", 0);
            var val2 = ExcelReaderHelper.ReadList(_objList, "Property3", 1);
            var val3 = ExcelReaderHelper.ReadList(_objList, "Property3", 2);
            var val4 = ExcelReaderHelper.ReadList(_objList, "Property3", 3);
            var val5 = ExcelReaderHelper.ReadList(_objList, "Property3", 4);
            var val6 = ExcelReaderHelper.ReadList(_objList, "Property3", 5);

            Assert.AreEqual(val, "7/14/2000 12:00:00 AM");
            Assert.AreEqual(val2, "7/14/2002 12:00:00 AM");
            Assert.AreEqual(val3, "4/10/1902 12:18:34 AM");
            Assert.AreEqual(val4, "12/6/2002 12:11:22 PM");
            Assert.AreEqual(val5, "12/6/2002 12:11:22 AM");
            Assert.AreEqual(val6, "11/18/2003 11:25:00 AM");
        }

        [Test]
        public void Test_SetValue_To_List_Column_Double()
        {
            var val = ExcelReaderHelper.ReadList(_objList, "Property4", 0);
            var val2 = ExcelReaderHelper.ReadList(_objList, "Property4", 1);
            var val3 = ExcelReaderHelper.ReadList(_objList, "Property4", 2);
            var val4 = ExcelReaderHelper.ReadList(_objList, "Property4", 3);
            var val5 = ExcelReaderHelper.ReadList(_objList, "Property4", 4);
            var val6 = ExcelReaderHelper.ReadList(_objList, "Property4", 5);

            Assert.AreEqual(val, "120000");
            Assert.AreEqual(val2, "52544");
            Assert.AreEqual(val3, "16013");
            Assert.AreEqual(val4, "-12354");
            Assert.AreEqual(val5, "12222");
            Assert.AreEqual(val6, "654");
        }
        [Test]
        public void Test_SetValue_To_List_Column_Float()
        {
            var val = ExcelReaderHelper.ReadList(_objList, "Property5", 0);
            var val2 = ExcelReaderHelper.ReadList(_objList, "Property5", 1);
            var val3 = ExcelReaderHelper.ReadList(_objList, "Property5", 2);
            var val4 = ExcelReaderHelper.ReadList(_objList, "Property5", 3);
            var val5 = ExcelReaderHelper.ReadList(_objList, "Property5", 4);
            var val6 = ExcelReaderHelper.ReadList(_objList, "Property5", 5);

            Assert.AreEqual(val, "12.3215");
            Assert.AreEqual(val2, "12.32");
            Assert.AreEqual(val3, "-56.369");
            Assert.AreEqual(val4, "-132.32");
            Assert.AreEqual(val5, "12321");
            Assert.AreEqual(val6, "654");
        }
        [Test]
        public void Test_SetValue_To_List_Column_int64()
        {
            var val = ExcelReaderHelper.ReadList(_objList, "Property6", 0);
            var val2 = ExcelReaderHelper.ReadList(_objList, "Property6", 1);
            var val3 = ExcelReaderHelper.ReadList(_objList, "Property6", 2);
            var val4 = ExcelReaderHelper.ReadList(_objList, "Property6", 3);
            var val5 = ExcelReaderHelper.ReadList(_objList, "Property6", 4);
            var val6 = ExcelReaderHelper.ReadList(_objList, "Property6", 5);

            Assert.AreEqual(val, "710");
            Assert.AreEqual(val2, "53119");
            Assert.AreEqual(val3, "26013");
            Assert.AreEqual(val4, "-123");
            Assert.AreEqual(val5, "32214254");
            Assert.AreEqual(val6, "684");
        }

        [Test]
        public void Test_SetValue_To_List_Column_Uint32()
        {
            var val = ExcelReaderHelper.ReadList(_objList, "Property7", 0);
            var val2 = ExcelReaderHelper.ReadList(_objList, "Property7", 1);
            var val3 = ExcelReaderHelper.ReadList(_objList, "Property7", 2);
            var val4 = ExcelReaderHelper.ReadList(_objList, "Property7", 3);
            var val5 = ExcelReaderHelper.ReadList(_objList, "Property7", 4);
            var val6 = ExcelReaderHelper.ReadList(_objList, "Property7", 5);

            Assert.AreEqual(val, "98721");
            Assert.AreEqual(val2, "3654");
            Assert.AreEqual(val3, "654");
            Assert.AreEqual(val4, "12354");
            Assert.AreEqual(val5, "12325");
            Assert.AreEqual(val6, "789");
        }

        [Test]
        public void Test_SetValue_To_List_Column_Decimal()
        {
            var val = ExcelReaderHelper.ReadList(_objList, "Property8", 0);
            var val2 = ExcelReaderHelper.ReadList(_objList, "Property8", 1);
            var val3 = ExcelReaderHelper.ReadList(_objList, "Property8", 2);
            var val4 = ExcelReaderHelper.ReadList(_objList, "Property8", 3);
            var val5 = ExcelReaderHelper.ReadList(_objList, "Property8", 4);
            var val6 = ExcelReaderHelper.ReadList(_objList, "Property8", 5);

            Assert.AreEqual(val, "1235");
            Assert.AreEqual(val2, "321.12");
            Assert.AreEqual(val3, "-654.35");
            Assert.AreEqual(val4, "-32165");
            Assert.AreEqual(val5, "12321.33");
            Assert.AreEqual(val6, "5441");
        }

        [Test]
        public void Test_SetValue_To_List_Column_Uint64()
        {
            var val = ExcelReaderHelper.ReadList(_objList, "Property9", 0);
            var val2 = ExcelReaderHelper.ReadList(_objList, "Property9", 1);
            var val3 = ExcelReaderHelper.ReadList(_objList, "Property9", 2);
            var val4 = ExcelReaderHelper.ReadList(_objList, "Property9", 3);
            var val5 = ExcelReaderHelper.ReadList(_objList, "Property9", 4);
            var val6 = ExcelReaderHelper.ReadList(_objList, "Property9", 5);

            Assert.AreEqual(val, "10132154");
            Assert.AreEqual(val2, "965464");
            Assert.AreEqual(val3, "456");
            Assert.AreEqual(val4, "123524164");
            Assert.AreEqual(val5, "1233214");
            Assert.AreEqual(val6, "123");
        }

        [Test]
        public void Test_SetValue_To_List_Column_int16()
        {
            var val = ExcelReaderHelper.ReadList(_objList, "Property10", 0);
            var val2 = ExcelReaderHelper.ReadList(_objList, "Property10", 1);
            var val3 = ExcelReaderHelper.ReadList(_objList, "Property10", 2);
            var val4 = ExcelReaderHelper.ReadList(_objList, "Property10", 3);
            var val5 = ExcelReaderHelper.ReadList(_objList, "Property10", 4);
            var val6 = ExcelReaderHelper.ReadList(_objList, "Property10", 5);

            Assert.AreEqual(val, "101");
            Assert.AreEqual(val2, "964");
            Assert.AreEqual(val3, "546");
            Assert.AreEqual(val4, "-123");
            Assert.AreEqual(val5, "1230");
            Assert.AreEqual(val6, "123");
        }

        [Test]
        public void Test_SetValue_To_List_Column_char()
        {
            var val = ExcelReaderHelper.ReadList(_objList, "Property11", 0);
            var val2 = ExcelReaderHelper.ReadList(_objList, "Property11", 1);
            var val3 = ExcelReaderHelper.ReadList(_objList, "Property11", 2);
            var val4 = ExcelReaderHelper.ReadList(_objList, "Property11", 3);
            var val5 = ExcelReaderHelper.ReadList(_objList, "Property11", 4);
            var val6 = ExcelReaderHelper.ReadList(_objList, "Property11", 5);

            Assert.AreEqual(val, "A");
            Assert.AreEqual(val2, "6");
            Assert.AreEqual(val3, "F");
            Assert.AreEqual(val4, "$");
            Assert.AreEqual(val5, "5");
            Assert.AreEqual(val6, "d");
        }
    }
}
