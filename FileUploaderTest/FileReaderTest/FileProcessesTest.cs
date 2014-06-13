using System.IO;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.AliasRecordCreator;
using ColloSys.FileUploader.FileReader;
using ColloSys.FileUploader.RecordCreator;
using ColloSys.FileUploader.RowCounter;
using ColloSys.FileUploader.Utilities;
using ColloSys.FileUploaderService.AliasRecordCreator;
using ColloSys.FileUploaderService.FileReader;
using ColloSys.FileUploaderService.RecordCreator;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

using ReflectionExtension.ExcelReader;

namespace ReflectionExtension.Tests.FileReaderTest
{
    [TestFixture]
    class FileProcessesTest
    {
        private FileProcess _fileProcess;
        private FileScheduler _fileScheduler;
        readonly FileDataProvider _obj = new FileDataProvider();
        private ICounter _counter;
        //SetUpAssembliesForTest objTest = new SetUpAssembliesForTest();
        private SchemaExport _db;

        [SetUp]
        public void Init()
        {

            _counter = new ExcelRecordCounter();
            _fileProcess = new FileProcess();

            _fileScheduler = _obj.GetUploadedFile();

        }

        [Test]
        public void Test_ComputeStatus_Assigning_ErrorRow_To_FileScheduler()
        {
            //Arrange
            _fileScheduler.ErrorRows = 1;
            _fileScheduler.ValidRows = 5;

            //Act
            _fileProcess.ComputeStatus(_fileScheduler, _counter);


            Assert.AreEqual(_fileScheduler.UploadStatus, ColloSysEnums.UploadStatus.DoneWithError);
        }

        [Test]
        public void Test_UpdateFileScheduler_Assigning_UploadStatus()
        {
            //Arrange
            _fileProcess.UpdateFileScheduler(_fileScheduler, _counter, ColloSysEnums.UploadStatus.DoneWithError);

            var fs = new FileScheduler();
            IExcelReader exreader = SharedUtility.GetInstance(new FileInfo(_fileScheduler.FileDirectory + @"\" + _fileScheduler.FileName));
            IAliasRecordCreator<Payment> objrecord = new RlsPaymentLinerRecordCreator(_fileScheduler);
            IRecord<Payment> record = new RecordCreator<Payment>(objrecord, exreader, _counter);
            // RlsPaymentLinerFileReader rls=new RlsPaymentLinerFileReader();

            //Act
            _fileProcess.UpdateFileScheduler(_fileScheduler, _counter, ColloSysEnums.UploadStatus.UploadRequest);

            //Assert
            Assert.AreEqual(fs.ValidRows, 10);
        }

        [Test]
        public void Test_UpdateFileStatus_Asssigning()
        {
            //Arrange
            var file = new FileScheduler() { FileDetail = { } };

            //Act
            _fileProcess.UpdateFileStatus(_fileScheduler, ColloSysEnums.UploadStatus.Done, _counter);

            //Assert
            Assert.AreEqual(_fileScheduler.UploadStatus, ColloSysEnums.UploadStatus.Done);
        }

    }


 
}
