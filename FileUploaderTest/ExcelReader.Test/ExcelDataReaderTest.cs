using NUnit.Framework;
using ReflectionExtension.ExcelReader;

namespace ReflectionExtension.Tests.ExcelReader.Test
{
    [TestFixture]
    class ExcelDataReaderTest :SetUpAssemblies
    {
        private IExcelReader _excel;
        [SetUp]
        public void Init()
        {
            _excel=new ExcelDataReader(FileStream);
        }

        #region:: Total Rows & Total Columns ::

        [Test]
        public void Test_totalRows()
        {
            Assert.AreEqual(_excel.TotalRows, 6);
        }

        [Test]
        public void Test_totalColumns()
        {
            Assert.AreEqual(_excel.TotalColumns, 11);
        }


        #endregion

        #region:: Test GetVal with colPos::
        [TestCase(1,"Skydiving")]
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
        public void Test_Read_firstRow_Each_Columns(int pos,string expected)
        {
           string val= _excel.GetValue((uint)pos);
            Assert.AreEqual(val,expected);
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

        #region Test Skip,GetList,Constructor::
        [Test]
        public void Test_Skip()
        {
             _excel.Skip(2);
             string val = _excel.GetValue(1);
            Assert.AreEqual(val, "algo");
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



        //[Test]
        //public void Test_GetList_For_ExcelDataReaders()
        //{
        //    var obj = new ConvertExcelToList<ExcelReaderHelper>(FileStream);
        //    var excelmaping = ExcelReaderHelper.GetMappingInfo();
        //    obj.GetList(excelmaping);
        //}

        [Test]
        public void Test_Constructor_With_FileInfo()
        {
            _excel = new ExcelDataReader(FileInfo);

            Assert.AreEqual(_excel.TotalRows, 6);
            Assert.AreEqual(_excel.TotalColumns, 11);
        }

        #endregion
    }
}
