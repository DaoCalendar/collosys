using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.RowCounter;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.AliasReaderTest
{
    [TestFixture]
    class RlsEPaymentLinerRecordCreatorTest : SetUpAssemblies
    {
        #region ::ctor::

        private RlsEPaymentLinerRecordCreator _rlsPaymentLiner;
        private IExcelReader _reader;
        private ICounter _counter;
        private IList<string> _ePaymentExcludeCode;
        private readonly List<string> _eWriteoff = new List<string>();
        private FileScheduler _uploadedFile;

        private FileMappingData _mappingData;

        [SetUp]
        public void Init()
        {
            _uploadedFile = new FileScheduler();
            _mappingData = new FileMappingData();
            _reader = new NpOiExcelReader(FileInfo);
            _counter = new ExcelRecordCounter();
            _ePaymentExcludeCode = new List<string>();
            _ePaymentExcludeCode = _mappingData.GetTransactionList();
            _rlsPaymentLiner = new RlsEPaymentLinerRecordCreator(_ePaymentExcludeCode, _uploadedFile, _eWriteoff);
        }

        #endregion


        #region :: TestCase for ComputtedSetter::

        [Test]
        public void Test_ComputedSetter_Assigning_List_To_ePaymentExcludedCode()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _rlsPaymentLiner.ComputedSetter(payment, _reader, _counter);

            //Assert
            Assert.AreEqual(payment.IsExcluded, true);
        }

        [Test]
        public void Test_ComputtedSetter_Assigning_ExcludeReason()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _rlsPaymentLiner.ComputedSetter(payment, _reader, _counter);

            //Assert
            Assert.AreEqual(payment.ExcludeReason, "TransCode : 204, and TransDesc : PARTIAL REPAYMENT - REVERSAL");
        }

        [Test]
        public void Test_ComputtedSetter_Assigning_DebiAmount()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _rlsPaymentLiner.ComputedSetter(payment, _reader, _counter);

            //Assert
            Assert.AreEqual(payment.IsDebit, true);
        }

        #endregion

        #region ::Test For CheckBasicField::

        [Test]
        public void Test_CheckBasicFields()
        {
            //Arrange
            var mapping = _mappingData.GetMappingForCheckbasicField();

            //Act
            _reader.Skip(2);
            var checkbasicField = _rlsPaymentLiner.CheckBasicField(_reader, _counter);

            //Assert
            Assert.AreEqual(checkbasicField, false);
        }

        [Test]
        public void Test_CheckBasicFields_Assigning_InvalidField()
        {
            //Arrange
            var mapping = _mappingData.GetMappingForCheckbasicField();

            //Act
            _reader.Skip(7);
            _rlsPaymentLiner.CheckBasicField(_reader, _counter);

            //Assert
            Assert.AreEqual(_counter.IgnoreRecord, 1);
        }


        [Test]
        public void Test_IsRecordValid_Assigning_InValidDate_To_TransDate()
        {
            //Arrange
            var getPayment = _mappingData.GetPaymentForIsRecordValid();

            //Act
            var isRecordTrue = _rlsPaymentLiner.IsRecordValid(getPayment);

            //Assert
            Assert.AreEqual(isRecordTrue, false);
        }

        #endregion
    }
}
