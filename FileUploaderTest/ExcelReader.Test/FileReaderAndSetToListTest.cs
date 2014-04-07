using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.FileUploader.ExcelReader.FileReader;
using FileUploader.ExcelReader;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;

namespace ReflectionExtension.Tests.ExcelReader.Test
{
    class FileReaderAndSetToListTest : SetUpAssemblies 
    {
        
        private ICounter _counter;
        private FileReader<Payment> _record;
        private IExcelReader _excelReader;
        private ExcelReaderHelper _excelReaderHelper;
        private Payment _obj;

        [SetUp]
        public void Init()
        {
            _record=new FileReader<Payment>();
            _obj = new Payment();
            _excelReaderHelper = new ExcelReaderHelper();
            _excelReader = _excelReaderHelper.GetInstance(FileInfo);
            _counter = new ExcelRecordCounter();
        }

        [Test]
        public void Test_ReadAndSaveBatch_ObjList_IsNot_Empty()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _excelReader.Skip(2);
            _record.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 18);
            Assert.AreNotEqual(_record.RecordList.Count, 0);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assiging_BatchSize_10()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _excelReader.Skip(2);
            _record.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 10);
            Assert.AreEqual(_record.RecordList.Count, 8);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assiging_BatchSize_GreaterThan_TotalRows()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _excelReader.Skip(2);
            _record.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 50);
            Assert.AreEqual(_record.RecordList.Count, _excelReader.TotalRows-2);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assiging_Empty_FileMapping()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D48");
            _excelReader.Skip(2);
            _record.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 50);
            Assert.AreEqual(_counter.TotalRecords,0);
        }

        [Test]
        public void Test_ListCount_Assigning_Different_Skip()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _excelReader.Skip(3);
            _record.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 10);
            Assert.AreEqual(_record.RecordList.Count, 7);
        }

        [Test]
        public void Test_ReadAndSaveBatch_LastRow_Value()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _excelReader.Skip(2);
            _record.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 10);
            Assert.AreEqual(_record.RecordList.ElementAt(6).DebitAmount, 65.18);
        }
    }
}
