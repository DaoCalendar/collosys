using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.AliasReaderTest
{
    class RlsPaymentLinerRecordCreatorTest : SetUpAssemblies
    {
        #region ::ctor::

        private RlsPaymentLinerRecordCreator _rlsPaymentLiner;
        private IExcelReader _reader;
        private FileScheduler _uploadedFile;
        private FileMappingData _mappingData;

        [SetUp]
        public void Init()
        {
            _uploadedFile = new FileScheduler();
            _mappingData = new FileMappingData();
            _reader = new NpOiExcelReader(FileInfo);
            //_uploadedFile = _mappingData.GetUploadedFile();
            _rlsPaymentLiner = new RlsPaymentLinerRecordCreator(_uploadedFile);
        }
        #endregion


        #region Test GetComputation

        [Test]
        public void Test_GetComputation_Assiging_TranCode_Check_IsDebit()
        {
            //Arrange
            var payment = _mappingData.GetPaymentForTransAmount();

            //Act
            _rlsPaymentLiner.GetComputations(payment, _reader);

            //Assert
            Assert.AreEqual(payment.IsDebit, false);
        }

        [Test]
        public void Test_GetComputation_Assigning_DebitAmount_Check_TransAmount()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
            _rlsPaymentLiner.GetComputations(payment, _reader);

            //Assert
            Assert.AreEqual(payment.TransAmount, 100);
        }

        [Test]
        public void Test_GetComputation_Assigning_DebitAmount_And_TransCode()
        {
            //Arrange
            var payment = _mappingData.GetPayment();

            //Act
          var isComputted=  _rlsPaymentLiner.GetComputations(payment, _reader);

            //Assert
            Assert.AreEqual(isComputted, true);
        }
        #endregion


        //#region ComputtedSetter TestCases

        //[Test]
        //public void Test__ComputtedSeeter_Assigning_FileDate_To_FileShedular()
        //{
        //    //Arrange
        //    var payment = _mappingData.GetPayment();

        //    //Act
        //    _rlsPaymentLiner.ComputedSetter(payment, _reader, _counter);

        //    //Assert
        //    Assert.AreEqual(payment.FileDate, Convert.ToDateTime("4/15/2014"));
        //}

        //[Test]
        //public void Test_ComputedSetter_Assigning_FileDetail_Check_IsDebited()
        //{
        //    //Arrange
        //    var payment = _mappingData.GetPayment();

        //    //Act
        //    _rlsPaymentLiner.ComputedSetter(payment, _reader, _counter);

        //    //Assert
        //    Assert.AreEqual(payment.IsDebit, true);
        //}

        //[Test]
        //public void Test_ComputedSetter_Assigning_FileDetail_Check_transAmount()
        //{
        //    //Arrange
        //    var payment = _mappingData.GetPayment();

        //    //Act
        //    _rlsPaymentLiner.ComputedSetter(payment, _reader, _counter);

        //    //Assert
        //    Assert.AreEqual(payment.TransAmount, 0);
        //}

        //[Test]
        //public void Test_ComputedSetter_Assigning_CreditAmount()
        //{
        //    //Arrange
        //    var payment = _mappingData.GetPayment();

        //    //Act
        //    _rlsPaymentLiner.ComputedSetter(payment, _reader, _counter);

        //    //Assert
        //    Assert.AreEqual(payment.TransAmount, 100);
        //}

        //#endregion


        //#region CheckBasicField

        //[Test]
        //public void Test_CheckBasicField_Assigning_Empty_String_to_LoanNumber()
        //{
        //    //Arrange
        //    //var mapping = _mappingData.GetMappings();

        //    //Act
        //    _reader.Skip(2);
        //    _rlsPaymentLiner.CheckBasicField(_reader, _counter);

        //    //Asserts
        //    Assert.AreEqual(_counter.IgnoreRecord, 1);
        //}

        //[Test]
        //public void Test_CheckBasicField_Assigning_Valid_LoanNumber()
        //{
        //    //Arrange
        //    //var mapping = _mappingData.GetMappings();

        //    //Act
        //    _reader.Skip(3);
        //    var chkBasicField = _rlsPaymentLiner.CheckBasicField(_reader, _counter);

        //    //Assert
        //    Assert.AreEqual(chkBasicField, true);
        //}

        //[Test]
        //public void Test_CheckBasicField_Assigning_InValid_LoanNumber()
        //{
        //    //Arrange
        //   // var mapping = _mappingData.GetMappings();

        //    //Act
        //    _reader.Skip(7);
        //    _rlsPaymentLiner.CheckBasicField(_reader, _counter);

        //    //Assert
        //    Assert.AreEqual(_counter.IgnoreRecord, 1);
        //}

        //#endregion


       
    }
}
