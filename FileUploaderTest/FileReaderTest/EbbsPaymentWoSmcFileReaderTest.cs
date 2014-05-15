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
    class EbbsPaymentWoSmcFileReaderTest
    {
        private IFileReader<Payment> _fileReader;
        private FileScheduler _uploadedFile;
        private FileMappingData _data;
        private List<string> _excludeCodes;

        [SetUp]
        public void Init()
        {
            _data = new FileMappingData();
            //_uploadedFile = _data.GetUploadedFileForWoSmc();
            _excludeCodes = _data.GetTransactionList();
            _fileReader = new EbbsPaymentWoSmcFileReader(_uploadedFile);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Check_Valid_List_Count()
        {
            //Act
            var list = _fileReader.GetNextBatch();

            //Assert
            Assert.AreEqual(list.Count,2);
        }

        [Test]
        public void Test_ReadAndSaveBatch_Check_AccNo()
        {
            //Act
            var list = _fileReader.GetNextBatch();

            //Assert
            Assert.AreEqual(list.ElementAt(1).AccountNo, "52205816238");

        }
    }
}
