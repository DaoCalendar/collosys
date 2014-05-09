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
            
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

        }

        [Test]
        public void Test_ReadAndSaveBatch_Assigning_Valid_ExcelFile()
        {
            var list = _fileReader.GetNextBatch();

            //Assert
            Assert.AreEqual(list.ElementAt(12).AccountNo, "49163353");
        }

        [Test]
        public void Test_ReadAndSaveBatch_Check_ListCount()
        {
            var list = _fileReader.GetNextBatch();

            //Assert
            Assert.AreEqual(list.Count,17);
        }

    }
}
