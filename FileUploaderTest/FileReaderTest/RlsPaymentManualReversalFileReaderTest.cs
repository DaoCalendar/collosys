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
    class RlsPaymentManualReversalFileReaderTest
    {
        private IFileReader<Payment> _fileReader;
        private FileScheduler _uploadedFile;
        private FileMappingData _data;
        [SetUp]
        public void Init()
        {
            _data = new FileMappingData();
            _uploadedFile = _data.GetUploadedFileForRlsPaymentManualReserval();
            _fileReader = new RlsPaymentManualReversalFileReader(_uploadedFile);
       }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_Valid_ExcelFile_CheckListCount()
        {
            //Arrange
     
            //Act
            _fileReader.ReadAndSaveBatch();

            //Assert
            Assert.AreEqual(_fileReader.List.Count, 71);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_Valid_ExcelFile_Check_AccNo()
        {
            //Arrange

            //Act
            _fileReader.ReadAndSaveBatch();

            //Assert
            Assert.AreEqual(_fileReader.List.ElementAt(70).AccountNo, "48331589");

        }

    }
}
