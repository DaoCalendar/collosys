using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.AliasRecordCreator;
using ColloSys.FileUploader.RowCounter;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.AliasReaderTest
{
    [TestFixture]
    class EbbsPaymentLinerRecordCreatorTest:SetUpAssembliesForEbbs
    {
        private AliasPaymentRecordCreator _recordCreator;
        private FileScheduler _fileScheduler;
        private FileMappingData _mappingData;
        private IExcelReader _reader;
        private ICounter _counter;
        private List<string> _ePaymentExcludeCodes;

        [SetUp]
        public void Init()
        {
            _mappingData=new FileMappingData();
            _counter=new ExcelRecordCounter();
            _fileScheduler = _mappingData.GetUploadedFile();
            _reader=new NpOiExcelReader(FileInfo);
            _ePaymentExcludeCodes = _mappingData.GetTransactionList();
            _recordCreator=new EbbsPaymentLinerRecordCreator(_fileScheduler,_ePaymentExcludeCodes); 
        }

        [Test]
        public void Test_GetComputation_Assigning_ExcludeCode_Check_IsDebit()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _recordCreator.GetComputations(payment,_reader);

            //Arrange
            Assert.AreEqual(payment.IsDebit,true);
        }

        [Test]
        public void Test_IsValidRecord_Assigning_Schedular_check_IgnoreRecord()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _recordCreator.IsRecordValid(payment, _counter);

            //Assert
            Assert.AreEqual(_counter.IgnoreRecord, 1);
        }

        [Test]
        public void Test_GetComputation_Assigning_FileSchedular_Check_IsExcluded()
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
            Assert.AreEqual(payment.ExcludeReason, "TransCode : 204, and TransDesc : PARTIAL REPAYMENT - REVERSAL");
        }
    }
}
