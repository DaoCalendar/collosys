using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasFileReader;
using ColloSys.FileUploader.FileReader;
using NUnit.Framework;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.FileReaderTest
{
    [TestFixture]
    class EbbsPaymentWoAutoFileReaderTest
    {
        private IFileReader<Payment> _fileReader;
        private FileScheduler _uploadedFile;
        private FileMappingData _data;
        private List<string> _excludeCodes;

        [SetUp]
        public void Init()
        {
            _data = new FileMappingData();
            _uploadedFile = _data.GetUploadedFileForEbbsAutoOd();
            _excludeCodes = _data.GetTransactionList();
            _fileReader = new EbbsPaymentWoAutoFileReader(_uploadedFile);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_Valid_ExcelFile()
        {
            //Arrange
            var payment = _data.GetPayment();
            var mappings = _data.GetMappings();

            //Act
            var list = _fileReader.GetNextBatch();

            //Assert
            Assert.AreEqual(list.ElementAt(1).AccountNo, "12345678905");
        }

        [Test]
        public void Test_ReadAndSaveBatch_Check_ListCount()
        {
            //Act
            var list = _fileReader.GetNextBatch();

            //Assert
            Assert.AreEqual(list.Count, 2);

        }
    }
}
