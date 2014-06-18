#region references

using System;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.RowCounter;
using ColloSys.FileUploaderService.AliasPayment;
using ColloSys.FileUploaderService.RecordManager;
using NSubstitute;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;

#endregion


namespace ReflectionExtension.Tests.AliasReaderTest
{
    [TestFixture]
    class AliasPaymentRecordCreatorTest : FileProvider
    {
        #region ctor

        private FileDataProvider _mappingData;
        private FileScheduler _fileScheduler;
        private RecordCreator<Payment> _objCreator;
        private IExcelReader _reader;
        private ICounter _counter;
        private Payment record;

        [SetUp]
        public void Init()
        {
            var fileDate = new DateTime();
            var dirPath = "";
            //Arrange
            _mappingData = new FileDataProvider(fileDate, dirPath);
            _reader = new NpOiExcelReader(FileInfoForDrillDown);
            _counter = new ExcelRecordCounter();
            var Scheduler = _mappingData.GetFileScheduler("DrillDown_Txn_1.xls",
                ColloSysEnums.FileAliasName.E_PAYMENT_LINER);
            record=new Payment();
            _objCreator = new EbbsPaymentLinerRecordCreator();
        }

        #endregion

       

        //[Test]
        //public void Test_CreateRecord_Assigning_ValidMapping_Check_AccNo()
        //{
        //    //Arrange
        //    var payment = _mappingData.GetUploadedFile();
        //    _objCreator.Init(payment, _counter, _reader);

        //    var mapping = _mappingData.GetFileMappingsForE_Payment_DrillDown();

        //    //Act
        //    _reader.Skip(3);
        //    _objCreator.CreateRecord(mapping, out record);

        //    //Assert
        //    Assert.AreEqual(record.AccountNo, "42297532");
        //}

        //[Test]
        //public void Test_CreateRecord_Assigning_ValidMapping_Check_()
        //{
        //    //Arrange
        //    var payment = _mappingData.GetUploadedFile();
        //    _objCreator.Init(payment, _counter, _reader);

        //    var mapping = _mappingData.GetFileMappingsForE_Payment_DrillDown();

        //    //Act
        //    _reader.Skip(3);
        //    _objCreator.CreateRecord(mapping, out record);

        //    //Assert
        //    Assert.AreEqual(record.TransDate, Convert.ToDateTime("2013-02-07"));
        //}

        //[Test]
        //public void Test_ExcelMapper_Assigning_Mapping_with_AccNo_Position()
        //{
        
        //    //Arrange
        //  var Scheduler = _mappingData.GetUploadedFile();
        //  _objCreator.Init(Scheduler, _counter, _reader);
        //    //Act
        //    _reader.Skip(3);
        //    _objCreator.ExcelMapper(record, Scheduler.FileDetail.FileMappings);

        //    //Assert
        //    Assert.AreEqual(record.DebitAmount, 6725);
        //}


        //[Test]
        //public void Test_ExcelMapper_Assigning_Mapping_with_TranceCode_Position()
        //{

        //    //Arrange
        //    var Scheduler = _mappingData.GetUploadedFile();

        //    _objCreator.Init(Scheduler, _counter, _reader);
        //    var mapping = _mappingData.getMappingForExcelmapper();
        //    //Act
        //    _reader.Skip(3);
        //    _objCreator.ExcelMapper(record, mapping);

        //    //Assert
        //    Assert.AreEqual(record.TransCode, 204);
        //}

        //[Test]
        //public void Test_Defaultmapper_Assigning_ValidMapping_Check_BilStatus()
        //{
        //    //Arrange
        //    var scheduler = _mappingData.GetUploadedFile();

        //    var map = _mappingData.getMappingForDefaultmapper();
               
        //    _objCreator.Init(scheduler, _counter, _reader);
      
        //    //Act
        //    _reader.Skip(3);
        //    _objCreator.DefaultMapper(record,map);

        //    //Assert
        //    Assert.AreEqual(record.BillStatus, ColloSysEnums.BillStatus.Unbilled);
        //}

        //[Test]
        //public void Test_ComputtedMapper_Assigning_Valid_Mapping()
        //{
        //    //Arrange
        //    var scheduler = _mappingData.GetUploadedFile();
        //    _objCreator.Init(scheduler, _counter, _reader);

        //    //Act
        //    _reader.Skip(2);
        //    bool isComputtedSetter = _objCreator.ComputedSetter(record);

        //    //Assert
        //    Assert.AreEqual(isComputtedSetter, true);

        //}

        //[Test]
        //public void Test_CheckBasicField_Assigning_Valid_AccounNo()
        //{
        //    //Arrange
        //    var scheduler = _mappingData.GetUploadedFile();
        //    _objCreator.Init(scheduler, _counter, _reader);


        //    //Act
        //    _reader.Skip(2);
        //    bool isValidBasicField = _objCreator.CheckBasicField();

        //    //Assert
        //    Assert.AreEqual(isValidBasicField, true);

        //}

        //[Test]
        //public void Test_CheckBasicField_Assignsing_InValid_AccounNo()
        //{
        //    //Arrange
        //    var scheduler = _mappingData.GetUploadedFile();
        //    _objCreator.Init(scheduler, _counter, _reader);


        //    //Act
        //    _reader.Skip(1);
        //    bool isValidBasicField = _objCreator.CheckBasicField();

        //    //Assert
        //    Assert.AreEqual(isValidBasicField, false);

        //}

        //[Test]
        //public void Test_CheckBasicField_Assigning_InValidAccountNo_Check_IgnoreCount()
        //{
        //    //Arrange
        //    var scheduler = _mappingData.GetUploadedFile();
        //    _objCreator.Init(scheduler, _counter, _reader);

        //    //Act
        //    _reader.Skip(7);
        //      _objCreator.CheckBasicField();

        //    //Assert
        //    Assert.AreEqual(_counter.IgnoreRecord, 1);
        //}

        //#region Computted
     



        //[Test]
        //public void Test_ComputtedSetter_Assigning_ValidPosition_For_AccountNo()
        //{
        //    //Arrange
        //    var payment = _mappingData.GetPayment();

        //    //Act
        //    _reader.Skip(3);
        //   // _objCreator.ComputedSetter(payment, _reader, _counter);

        //    //Assert
        //    Assert.AreEqual(payment.AccountNo, "42297532");

        //}

        //#endregion

        #region checkBasicField Test

        [Test]
        public void Test_CheckBasicField_Assigning_Valid_AccountNo()
        {
            //Arrange


            //Act
            _reader.Skip(3);
          // var isValidBasicField = _objCreator.CheckBasicField(_reader, _counter);

            //Assert
       //     Assert.AreEqual(isValidBasicField, true);
        }

        [Test]
        public void Test_CheckBasicField_Assigning_InValid_AccountNo()
        {
            //Arrange

            //Act
            _reader.Skip(7);
          //  var isValidBasicField = _objCreator.CheckBasicField(_reader, _counter);

            //Assert
          //  Assert.AreEqual(isValidBasicField, false);
        }

      

        #endregion

    }

}
