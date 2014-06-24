using System.IO;
using ColloSys.FileUploaderService.ExcelReader;
using NUnit.Framework;
using FileUploader.ExcelReader;
using ReflectionExtension.ExcelReader;


namespace ReflectionExtension.Tests.ExcelReader.Test
{
    [TestFixture]
    internal class EpPlusExcelReaderTest
    {
        private IExcelReader _excel;
        private FileInfo _fileInfo;

        [SetUp]
        public void Init()
        {
            _fileInfo = new FileInfo("DrillDown_Txn_1.xls");
            _excel = new EpPlusExcelReader(_fileInfo);
        }

        #region::Assigning Diffrent files to Constructor::

        [Test]
        public void Assigning_xlsx_File_To_FileInfo()
        {
            Assert.AreNotEqual(_excel.TotalRows, 0);
        }

        [Test]
        public void Assigning_FileInfo_To_EpPlusConstructor()
        {
            var fileInfo = new FileInfo("c:\\ExampleData.xlsx");
            _excel = new EpPlusExcelReader(fileInfo);
            Assert.AreNotEqual(_excel.TotalRows, 0);
        }

        [Test]
        public void Test_GetList_For_EpPlusReader()
        {
            var obj = new ConvertExcelToList<ExcelReaderHelper>(_fileInfo);
            var excelmaping = ExcelReaderHelper.GetMappingInfo();
            obj.GetList(excelmaping);
        }



        #endregion

        #region::Read Excel ColumnsWise::
        
        [TestCase(1, 1, "Skydiving")]
        [TestCase(1, 2, "Elevator")]
        [TestCase(1, 3, "algo")]
        [TestCase(1, 4, "negativeNumbers")]
        [TestCase(1, 5, "Thousands")]
        [TestCase(1, 6, "non Numbric string")]
        public void Read_String_Column_Values(int colPos, int rowPos, string expected)
        {
            string value = _excel.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }

        [TestCase(2, 1, "710")]
        [TestCase(2, 2, "575")]
        [TestCase(2, 3, "10000")]
        [TestCase(2, 4, "(3214)")]
        [TestCase(2, 5, "123321")]
        [TestCase(2, 6, "123")]
        public void Read_Int32_Column_Values(int colPos, int rowPos, string expected)
        {
            string value = _excel.GetValue((uint)rowPos, (uint)colPos);
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
            string value = _excel.GetValue((uint)rowPos, (uint)colPos);
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
            string value = _excel.GetValue((uint)rowPos, (uint)colPos);
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
            string value = _excel.GetValue((uint)rowPos, (uint)colPos);
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
            string value = _excel.GetValue((uint)rowPos, (uint)colPos);
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
            string value = _excel.GetValue((uint)rowPos, (uint)colPos);
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
            string value = _excel.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }

        [TestCase(9, 1, "10132154")]
        [TestCase(9, 2, "965464")]
        [TestCase(9, 3, "456")]
        [TestCase(9, 4, "123524164")]
        [TestCase(9, 5, "1233214")]
        [TestCase(9, 6, "123")]
        public void Read_Uint64_Column_Values(int colPos, int rowPos, string expected)
        {
            string value = _excel.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }

        [TestCase(10, 1, "101")]
        [TestCase(10, 2, "964")]
        [TestCase(10, 3, "546")]
        [TestCase(10, 4, "(123)")]
        [TestCase(10, 5, "123,0")]
        [TestCase(10, 6, "123")]
        public void Read_int16_Column_Values(int colPos, int rowPos, string expected)
        {
            string value = _excel.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }

        [TestCase(11, 1, "A")]
        [TestCase(11, 2, "6")]
        [TestCase(11, 3, "F")]
        [TestCase(11, 4, "$")]
        [TestCase(11, 5, "5")]
        [TestCase(11, 6, "d")]
        public void Read_Character_Column_Values(int colPos, int rowPos, string expected)
        {
            string value = _excel.GetValue((uint)rowPos, (uint)colPos);
            Assert.AreEqual(value, expected);
        }


        #endregion

        #region ::GetVal With ColPos::
        [TestCase(1, "Skydiving")]
        [TestCase(2, "710")]
        [TestCase(3, "14/7/2000")]
        [TestCase(4, "1,20,000")]
        [TestCase(5, "12.3215")]
        [TestCase(6, "710")]
        [TestCase(7, "98721")]
        [TestCase(8, "1235")]
        [TestCase(9, "10132154")]
        [TestCase(10, "101")]
        [TestCase(11, "A")]
        public void Test_Read_firstRow_Each_Columns(int pos, string expected)
        {
            string val = _excel.GetValue((uint)pos);
            Assert.AreEqual(val, expected);
        }

        [TestCase(1, "Elevator")]
        [TestCase(2, "575")]
        [TestCase(3, "14,7,2002")]
        [TestCase(4, "52544")]
        [TestCase(5, "12.32")]
        [TestCase(6, "53119")]
        [TestCase(7, "3654")]
        [TestCase(8, "321.12")]
        [TestCase(9, "965464")]
        [TestCase(10, "964")]
        [TestCase(11, "6")]
        public void Test_Read_SecoundRow_Each_Columns(int pos, string expected)
        {
            _excel.NextRow();
            string val = _excel.GetValue((uint)pos);
            Assert.AreEqual(val, expected);
        }

        [TestCase(1, "algo")]
        [TestCase(2, "10000")]
        [TestCase(3, "831.01289631332")]
        [TestCase(4, "16013")]
        [TestCase(5, "-56.369")]
        [TestCase(6, "26013")]
        [TestCase(7, "654")]
        [TestCase(8, "-654.35")]
        [TestCase(9, "456")]
        [TestCase(10, "546")]
        [TestCase(11, "F")]
        public void Test_Read_ThirdRow_Each_Columns(int pos, string expected)
        {
            _excel.NextRow();
            _excel.NextRow();
            string val = _excel.GetValue((uint)pos);
            Assert.AreEqual(val, expected);
        }

        #endregion

        #region:: Test Skip Fuction::

        [Test]
        public void Assigning_Count_To_Skip()
        {
            _excel.Skip(3);
            Assert.AreEqual(_excel.CurrentRow, 4);
        }

        [Test]
        public void Assigning_Count_GreaterThan_TotalRows_To_Skip()
        {
            _excel.Skip(7);
            Assert.AreEqual(_excel.CurrentRow, 6);
        }

        [Test]
        public void Assigning_Count_After_Reading_FirstRows_To_Skip()
        {
            _excel.NextRow();
            _excel.Skip(3);
            Assert.AreEqual(_excel.CurrentRow, 5);
        }

        #endregion


    }
}
