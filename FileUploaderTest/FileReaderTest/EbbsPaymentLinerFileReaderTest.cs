using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasFileReader;
using ColloSys.FileUploader.FileReader;
using NUnit.Framework;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.FileReaderTest
{
    [TestFixture]
    class EbbsPaymentLinerFileReaderTest
    {
        private IFileReader<Payment> _fileReader;
        private FileScheduler _uploadedFile;
        private FileMappingData _data;
        private List<string> _excludeCodes;
        
        [SetUp]
        public void Init()
        {
            _data = new FileMappingData();
            _uploadedFile = _data.GetUploadedFile();
            _excludeCodes = _data.GetTransactionList();
            _fileReader = new EbbsPaymentLinerFileReader(_uploadedFile,_excludeCodes);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_Valid_ExcelFile()
        {
            //Arrange
            var payment = _data.GetPayment();
            var mappings = _data.GetMappings();

            //Act
            _fileReader.ReadAndSaveBatch();

            //Assert
            Assert.AreEqual(payment.AccountNo, "");
        }

        [Test]
        public void Test_ReadAndSaveBatch_Check_ListCount()
        {
            //Arrange
            var payment = _data.GetPayment();
            var mappings = _data.GetMappings();

            //Act
            _fileReader.ReadAndSaveBatch( );

            //Assert
            Assert.AreEqual(_fileReader.List.Count, 14);

        }
    }
}
