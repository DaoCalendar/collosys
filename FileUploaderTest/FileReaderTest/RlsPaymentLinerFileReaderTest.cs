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
    class RlsPaymentLinerFileReaderTest 
    {
        private IFileReader<Payment> _fileReader;
        private FileScheduler _uploadedFile;
        private FileMappingData _data;

        [SetUp]
        public void Init()
        {
            _data=new FileMappingData();
            _uploadedFile = _data.GetUploadedFile();
            _fileReader = new RlsPaymentLinerFileReader(_uploadedFile);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_Valid_ExcelFile1()
        {
            //Arrange
            var payment = _data.GetPayment();

            //Act
            _fileReader.ReadAndSaveBatch();

            //Assert
            Assert.AreEqual(_fileReader.List.ElementAt(12).AccountNo, "49163353");
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_Valid_ExcelFile()
        {
            //Arrange
            var payment = _data.GetPayment();
            
            //Act
            _fileReader.ReadAndSaveBatch();

            //Assert
            Assert.AreEqual(_fileReader.List.ElementAt(12).AccountNo, "49163353");
        }

        [Test]
        public void Test_ReadAndSaveBatch_Check_ListCount()
        {
            //Arrange
           
            //Act
            _fileReader.ReadAndSaveBatch();

            //Assert
            Assert.AreEqual(_fileReader.List.Count,17);
        }

    }
}
