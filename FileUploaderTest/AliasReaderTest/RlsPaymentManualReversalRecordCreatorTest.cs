using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.AliasRecordCreator;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.AliasReaderTest
{
    [TestFixture]
    class RlsPaymentManualReversalRecordCreatorTest :SetUpAssemblies
    {
        private AliasPaymentRecordCreator _objRecordCreator;
        private FileScheduler _fileUploaded;
        private IExcelReader _reader;
        private FileMappingData _mappingData;
        
        [SetUp]
        public void Init()
        {
            _mappingData=new FileMappingData();
            _fileUploaded = _mappingData.GetUploadedFile();
            _reader=new NpOiExcelReader(FileInfo);
            _objRecordCreator=new RlsPaymentManualReversalRecordCreator(_fileUploaded);
        }

        #region Test GetComputation

        [Test]
        public void Test_GetComputation()
        {
            //Arrange
            var payment = _mappingData.GetPaymentForTransAmount();

            //Act
            _objRecordCreator.GetComputations(payment, _reader);

            //Assert
            Assert.AreEqual(payment.IsDebit, true);
        }

        [Test]
        public void Test_GetComputation_Assigning_TransAmount_null()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _objRecordCreator.GetComputations(payment, _reader);

            //Assert
            Assert.AreEqual(payment.IsDebit, false);
        }

        #endregion

    }
}
