using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasFileReader;
using ColloSys.FileUploader.FileReader;
using ColloSys.FileUploaderService.AliasPayment;
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
          //  _uploadedFile = _data.GetUploadedFileForRlsPaymentManualReserval();
            _fileReader = new RlsPaymentManualReversalFileReader(_uploadedFile);
       }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_Valid_ExcelFile_CheckListCount()
        {
            var list = _fileReader.GetNextBatch();

            //Assert
            Assert.AreEqual(list.Count, 71);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_Valid_ExcelFile_Check_AccNo()
        {
            var list = _fileReader.GetNextBatch();

            //Assert
            Assert.AreEqual(list.ElementAt(70).AccountNo, "48331589");

        }

    }
}
