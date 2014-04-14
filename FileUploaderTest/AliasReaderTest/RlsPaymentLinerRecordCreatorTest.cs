using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.RowCounter;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.AliasReaderTest
{
    class RlsPaymentLinerRecordCreatorTest :SetUpAssemblies
    {
        #region ::ctor::

        private RlsPaymentLinerRecordCreator _rlsPaymentLiner;
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

            _uploadedFile.FileDetail = _mappingData.GetFileDetail();
            _rlsPaymentLiner = new RlsPaymentLinerRecordCreator(_uploadedFile);
        }

        [Test]
        public void Test_ComputedSetter_Assigning_FileDetail_Check_IsDebited()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _rlsPaymentLiner.ComputedSetter(payment);

            //Assert
            Assert.AreEqual(payment.IsDebit, true);
        }

        [Test]
        public void Test_ComputedSetter_Assigning_FileDetail_Check_transAmount()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _rlsPaymentLiner.ComputedSetter(payment);

            //Assert
            Assert.AreEqual(payment.TransAmount, 0);
        }

        [Test]
        public void Test_ComputedSetter_Assigning_CreditAmount()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _rlsPaymentLiner.ComputedSetter(payment);

            //Assert
            Assert.AreEqual(payment.TransAmount,100);
        }

        [Test]
        public void Test_CheckBasicField()
        {
            
        }

        #endregion
    }
}
