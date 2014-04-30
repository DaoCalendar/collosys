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
    class RlsPaymentWoAebFileReaderTest
    {
        private IFileReader<Payment> _fileReader;
        private FileScheduler _uploadedFile;
        private FileMappingData _data;
        [SetUp]
        public void Init()
        {
            _data = new FileMappingData();
            _uploadedFile = _data.GetUploadedFileForRlsPaymentWoAeb();
            _fileReader = new RlsPaymentWoAebFileReader(_uploadedFile);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_Valid_ExcelFile()
        {
            //Arrange

            //Act
            _fileReader.ReadAndSaveBatch();

            //Assert
            Assert.AreEqual(_fileReader.List.ElementAt(11).AccountNo, "48547727");
        }

        [Test]
        public void Test_ReadAndSaveBatch_Check_ListCount()
        {
            //Arrange

            //Act
            _fileReader.ReadAndSaveBatch();

            //Assert
            Assert.AreEqual(_fileReader.List.Count, 12);

        }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_BatchSize_Is_7()
        {
            //Arrange
            var payment = _data.GetPayment();
            var mappings = _data.GetMappings();

            //Act
            _fileReader.ReadAndSaveBatch();

            //Assert
            Assert.AreEqual(_fileReader.List.ElementAt(0).AccountNo, "48080063");

        }
    }
}
