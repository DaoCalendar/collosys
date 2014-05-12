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
using NSubstitute;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.FileReaderTest
{
    [TestFixture]
    class FileProcessesTest
    {
        private FileProcess _fileProcess;
        private FileScheduler _fileScheduler;
        readonly FileMappingData _obj = new FileMappingData();
        private ICounter _counter;
        

        [SetUp]
        public void Init()
        {
            _fileScheduler = _obj.GetUploadedFile();
            _counter = new ExcelRecordCounter();
        }

        [Test]
        public void Test_ComputeStatus_Assigning_ErrorRow_To_FileScheduler()
        {
            //Arrange
            _fileScheduler.ErrorRows = 1;

            //Act
            _fileProcess.ComputeStatus(_fileScheduler);


            Assert.AreEqual(_fileScheduler.UploadStatus, ColloSysEnums.UploadStatus.DoneWithError);
        }

        [Test]
        public void Test_UpdateFileScheduler_Assigning_UploadStatus()
        {
            //Arrange
             _fileProcess.UpdateFileScheduler(_fileScheduler,_counter,ColloSysEnums.UploadStatus.DoneWithError);

            var fs = new FileScheduler();
            IExcelReader Exreader = SharedUtility.GetInstance(new FileInfo(_fileScheduler.FileDirectory + @"\" + _fileScheduler.FileName));
            IAliasRecordCreator<Payment> objrecord = new RlsPaymentLinerRecordCreator(_fileScheduler);
            IRecord<Payment> record = new RecordCreator<Payment>(objrecord, Exreader, _counter);
             // RlsPaymentLinerFileReader rls=new RlsPaymentLinerFileReader();
           
            //Act
            _fileProcess.UpdateFileScheduler(_fileScheduler, _counter, ColloSysEnums.UploadStatus.UploadRequest);

            //Assert
            Assert.AreEqual(fs.ValidRows, 10);
        }

        [Test]
        public void Test_UpdateFileStatus_Asssigning()
        {
        }
    }
}
