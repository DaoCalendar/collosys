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
    internal class RlsPaymentWoPlpcFileReaderTest
    {
        private IFileReader<Payment> _fileReader;
        private FileScheduler _uploadedFile;
        private FileMappingData _data;

        [SetUp]
        public void Init()
        {
            _data = new FileMappingData();
           // _uploadedFile = _data.GetUploadedFile();
            _fileReader = new RlsPaymentWoPlpcFileReader(_uploadedFile);
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
            var list = _fileReader.GetNextBatch();

            //Assert
            Assert.AreEqual(list.Count, 16);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_BatchSize_Is_7()
        {
            var list = _fileReader.GetNextBatch();

            //Assert
            Assert.AreEqual(list.ElementAt(0).AccountNo, "49163353");
        }
    }
}
