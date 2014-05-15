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
    class EbbsPaymentLinerFileReaderTest
    {
        private IFileReader<Payment> _fileReader;
        private FileScheduler _uploadedFile;
        private FileMappingData _data;
        
        [SetUp]
        public void Init()
        {
            _data = new FileMappingData();
            //_uploadedFile = _data.GetUploadedFileForEbbs();
            
            _fileReader = new EbbsPaymentLinerFileReader(_uploadedFile);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_Valid_ExcelFile()
        {
            //Arrange
            

            //Act
            var list = _fileReader.GetNextBatch();

            //Assert
            Assert.AreEqual(list.ElementAt(5).AccountNo, "52205836441");
        }

        [Test]
        public void Test_ReadAndSaveBatch_Check_ListCount()
        {
            //Arrange
          

            //Act
            var list = _fileReader.GetNextBatch();

            //Assert
            Assert.AreEqual(list.Count, 2);

        }
    }
}
