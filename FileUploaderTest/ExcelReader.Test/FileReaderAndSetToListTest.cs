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
        private ColloSys.FileUploader.ExcelReader.FileReader.FileReader<Payment> _fileReader;
        private IExcelReader _excelReader;
        private ExcelReaderHelper _excelReaderHelper;
        private Payment _obj;

        [SetUp]
        public void Init()
        {
            _fileReader= new ColloSys.FileUploader.ExcelReaders.FileReader.RlsPaymentFileReader();
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
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 18);
            Assert.AreNotEqual(_fileReader.RecordList.Count, 0);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assiging_BatchSize_10()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _excelReader.Skip(2);
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 10);
            Assert.AreEqual(_fileReader.RecordList.Count, 8);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assiging_BatchSize_GreaterThan_TotalRows()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _excelReader.Skip(2);
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 50);
            Assert.AreEqual(_fileReader.RecordList.Count, _excelReader.TotalRows-2);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assiging_Empty_FileMapping()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D48");
            _excelReader.Skip(2);
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 50);
            Assert.AreEqual(_counter.TotalRecords,0);
        }

        [Test]
        public void Test_ListCount_Assigning_Different_Skip()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _excelReader.Skip(3);
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 10);
            Assert.AreEqual(_fileReader.RecordList.Count, 7);
        }

        [Test]
        public void Test_ReadAndSaveBatch_LastRow_Value()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _excelReader.Skip(2);
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 10);
            Assert.AreEqual(_fileReader.RecordList.ElementAt(6).DebitAmount, 65.18);
        }

        [Test]
        public void Test_ReadAndSaveBatch_LastRow_Value_IncresingSkip()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _excelReader.Skip(3);
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 10);
            Assert.AreEqual(_fileReader.RecordList.ElementAt(5).DebitAmount, 65.18);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Row_Value_Of_InvalidRecord()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _excelReader.Skip(2);
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data, _counter, 20);
            Assert.AreEqual(_fileReader.RecordList.ElementAt(0).DebitAmount, 0);
        }
    
    }
}
