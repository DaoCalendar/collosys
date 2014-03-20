using System.IO;
using NUnit.Framework;
using FileUploader.ExcelReader;

namespace FileUploader.Tests.ExcelReader.Test
{
    [TestFixture]
    class NpOiExcelReaderTest
    {
        private NpOiExcelReader _npOiExcelReader;
        //private ConvertExcelToList<ExcelReaderHelper> _data;
       
        [SetUp]
        public void Init()
        {
            var fileInfo = new FileInfo("c:\\ExampleData.xls");
            _npOiExcelReader = new NpOiExcelReader(fileInfo);
           // _data=new ConvertExcelToList<ExcelReaderHelper>(_npOiExcelReader);
        }

        #region::Assiging Diffrent Files To constructor ::

        [Test]
        public void Assigning_xls_File_to_FileStream()
        {
            var fileInfo = new FileInfo(@"c:\\ExampleData.xls");
            _npOiExcelReader = new NpOiExcelReader(fileInfo);
            Assert.IsNotNull(_npOiExcelReader.TotalRows);
        }

        [Test]
        [ExpectedException]
        public void Assigning_Empty_xls_File_To_FileStream_ThrowsException()
        {
            var fileInfo = new FileInfo(@"c:\\Empty.xls");
            _npOiExcelReader = new NpOiExcelReader(fileInfo);
        }

        [Test]
        [ExpectedException]
        public void Assigning_Docx_File_To_FileStream_ThrowsException()
        {
            var fileInfo = new FileInfo("c:\\abc.docx");
            _npOiExcelReader = new NpOiExcelReader(fileInfo);
        }

        [Test]
        [ExpectedException]
        public void Assigning_NotPresented_File_To_FileInfo_ThrowsException()
        {
            var fileInfo = new FileInfo("c:\\ExampleData123.xls");
            _npOiExcelReader = new NpOiExcelReader(fileInfo);
        }


        #endregion

        //[TestCase(1, 1, "Skydiving", "Property1")]
        //public void Test(int rowPos, int colPos, string value, string propertyName)
        //{
        //    var fileInfo = new FileInfo("c:\\ExampleData.xls");
        //    _npOiExcelReader = new NpOiExcelReader(fileInfo);
            
        //  _data.SetValue(propertyName,   1, 1);

        //    Assert.AreEqual(value, _npOiExcelReader.GetValue((uint)colPos, (uint)rowPos));

        //}
        #region:: ColumnWise Test Case ::

        [TestCase(1, 1, "Skydiving")]
        [TestCase(1, 2, "Elevator")]
        [TestCase(1, 3, "algo")]
        [TestCase(1, 4, "negativeNumbers")]
        [TestCase(1, 5, "Thousands")]
        [TestCase(1, 6, "non Numbric string")]
        public void Read_String_Column_Values(int colPos, int rowPos, string expected)
        {
            _npOiExcelReader.NextRow();
            string value = _npOiExcelReader.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }

        [TestCase(2, 1, "710")]
        [TestCase(2, 2, "575")]
        [TestCase(2, 3, "10000")]
        [TestCase(2, 4, "(3214)")]
        [TestCase(2, 5, "123321")]
        [TestCase(2, 6, "asdf")]
        public void Read_Int32_Column_Values(int colPos, int rowPos, string expected)
        {
            _npOiExcelReader.NextRow();
            string value = _npOiExcelReader.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }

        [TestCase(3, 1, "14/7/2000")]
        [TestCase(3, 2, "14,7,2002")]
        [TestCase(3, 3, "831.01289631332")]
        [TestCase(3, 4, "6.12.2002 12:11:22")]
        [TestCase(3, 5, "6-12-2002 12:11:22 AM")]
        [TestCase(3, 6, "18-Nov-03 11:25 ")]
        public void Read_DateTime_Column_Values(int colPos, int rowPos, string expected)
        {
            _npOiExcelReader.NextRow();
            string value = _npOiExcelReader.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }

        [TestCase(4, 1, "1,20,000")]
        [TestCase(4, 2, "52544")]
        [TestCase(4, 3, "16013")]
        [TestCase(4, 4, "(123,54)")]
        [TestCase(4, 5, "12222")]
        [TestCase(4, 6, "654")]
        public void Read_Double_Column_Values(int colPos, int rowPos, string expected)
        {
            _npOiExcelReader.NextRow();
            string value = _npOiExcelReader.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }
        [TestCase(5, 1, "12.3215")]
        [TestCase(5, 2, "12.32")]
        [TestCase(5, 3, "-56.369")]
        [TestCase(5, 4, "(132.32)")]
        [TestCase(5, 5, "12,321")]
        [TestCase(5, 6, "654")]
        public void Read_Float_Column_Values(int colPos, int rowPos, string expected)
        {
            _npOiExcelReader.NextRow();
            string value = _npOiExcelReader.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }

        [TestCase(6, 1, "710")]
        [TestCase(6, 2, "53119")]
        [TestCase(6, 3, "26013")]
        [TestCase(6, 4, "(123)")]
        [TestCase(6, 5, "32214254")]
        [TestCase(6, 6, "684")]
        public void Read_Int64_Column_Values(int colPos, int rowPos, string expected)
        {
            _npOiExcelReader.NextRow();
            string value = _npOiExcelReader.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }

        [TestCase(7, 1, "98721")]
        [TestCase(7, 2, "3654")]
        [TestCase(7, 3, "654")]
        [TestCase(7, 4, "12354")]
        [TestCase(7, 5, "12,325")]
        [TestCase(7, 6, "789")]
        public void Read_Uint32_Column_Values(int colPos, int rowPos, string expected)
        {
            _npOiExcelReader.NextRow();
            string value = _npOiExcelReader.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }

        [TestCase(8, 1, "1235")]
        [TestCase(8, 2, "321.12")]
        [TestCase(8, 3, "-654.35")]
        [TestCase(8, 4, "(32165)")]
        [TestCase(8, 5, "12321.33")]
        [TestCase(8, 6, "5441")]
        public void Read_Decimal_Column_Values(int colPos, int rowPos, string expected)
        {
            _npOiExcelReader.NextRow();
            string value = _npOiExcelReader.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }

        [TestCase(9, 1, "10132154")]
        [TestCase(9, 2, "-965464")]
        [TestCase(9, 3, "asd456")]
        [TestCase(9, 4, "(123524164)")]
        [TestCase(9, 5, "1233214")]
        [TestCase(9, 6, "5441")]
        public void Read_Uint64_Column_Values(int colPos, int rowPos, string expected)
        {
            _npOiExcelReader.NextRow();
            string value = _npOiExcelReader.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }
        [TestCase(10, 1, "101")]
        [TestCase(10, 2, "-964")]
        [TestCase(10, 3, "#546")]
        [TestCase(10, 4, "(123)")]
        [TestCase(10, 5, "123,0")]
        [TestCase(10, 6, "123")]
        public void Read_int16_Column_Values(int colPos, int rowPos, string expected)
        {
            _npOiExcelReader.NextRow();
            string value = _npOiExcelReader.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }

        [TestCase(11, 1, "A")]
        [TestCase(11, 2, "#")]
        [TestCase(11, 3, "F")]
        [TestCase(11, 4, "$")]
        [TestCase(11, 5, "5")]
        [TestCase(11, 6, "213")]
        public void Read_Character_Column_Values(int colPos, int rowPos, string expected)
        {
            _npOiExcelReader.NextRow();
            string value = _npOiExcelReader.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }

        #endregion



    }
}
