using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.AliasReaderTest
{
    [TestFixture]
    class EbbsPaymentWoSmcRecordCreatorTest:SetUpAssemblies
    {
        private AliasPaymentRecordCreator _recordCreator;
        private FileScheduler _fileScheduler;
        private FileMappingData _mappingData;
        private IExcelReader _reader;
        private List<string> _ePaymentExcludeCodes;

        [SetUp]
        public void Init()
        {
            _mappingData = new FileMappingData();
            _fileScheduler = _mappingData.GetUploadedFile();
            _reader = new NpOiExcelReader(FileInfo);
            _ePaymentExcludeCodes = _mappingData.GetTransactionList();
            _recordCreator = new EbbsPaymentWoSmcRecordCreator(_fileScheduler, _ePaymentExcludeCodes);
        }

        [Test]
        public void Test_GetComputation_Assigning_ExcludeCode_Check_IsExcluded()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _recordCreator.GetComputations(payment, _reader);

            //Arrange
            Assert.AreEqual(payment.IsExcluded, true);
        }

        [Test]
        public void Test_GetComputation_Assigning_ExcludeCode_Check_ExcludeResion()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _recordCreator.GetComputations(payment, _reader);

            //Arrange
            Assert.AreEqual(payment.ExcludeReason, "Trans Code : 204, Desc : PARTIAL REPAYMENT - REVERSAL");
        }
    }
}
