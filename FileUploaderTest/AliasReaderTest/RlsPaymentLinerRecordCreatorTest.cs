using System;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.AliasPayment;
using ColloSys.FileUploaderService.FileReader;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;
using ReflectionExtension.Tests.DataCreator.FileUploader;

namespace ReflectionExtension.Tests.AliasReaderTest
{
    class RlsPaymentLinerRecordCreatorTest : FileProvider
    {
        #region ctor

        private IFileReader<Payment> _paymentLiner;
        Payment _record = new Payment();

        [SetUp]
        public void Init()
        {
            var fileDate = new DateTime();
            var dirPath = "";
            var mappingData = new FileDataProvider(fileDate,dirPath);
            var fileScheduler = mappingData.GetUploadedFile(ColloSysEnums.FileAliasName.R_PAYMENT_LINER);
            _paymentLiner = new RlsPaymentLinerFileReader(fileScheduler);

        }

        #endregion

        #region Test Case

        [Test]
        public void Test_CreateRecord_Assigning_ValidMapping_Check_AccNo()
        {
            _paymentLiner.ExcelReader.Skip(3);
            _paymentLiner.ObjRecord.CreateRecord(_paymentLiner._fs.FileDetail.FileMappings, out _record);

            //Assert
            Assert.AreEqual(_record.AccountNo, "47502703");
        }

        [Test]
        public void Test_CreateRecord_Assigning_ValidMapping_Check_()
        {
            _paymentLiner.ExcelReader.Skip(3);
            _paymentLiner.ObjRecord.CreateRecord(_paymentLiner._fs.FileDetail.FileMappings, out _record);


            //Assert
            Assert.AreEqual(_record.TransAmount, 5000);
        }

        [Test]
        public void Test_CreateRecord_Assigning_InValid_FileDate()
        {
            //Act
            _paymentLiner.ExcelReader.Skip(1);
            var isRecordValid = _paymentLiner.ObjRecord.CreateRecord(_paymentLiner._fs.FileDetail.FileMappings, out _record);

            //Assert
            Assert.AreEqual(isRecordValid, false);
        }

        [Test]
        public void Test_ExcelMapper_Assigning_Mapping_with_AccNo_Position()
        {
            _paymentLiner.ExcelReader.Skip(3);
            var excelMap =
               _paymentLiner._fs.FileDetail.FileMappings.Where(
                   x => x.ValueType == ColloSysEnums.FileMappingValueType.ExcelValue).ToList();
            _paymentLiner.ObjRecord.ExcelMapper(_record, excelMap);

            //Assert
            Assert.AreEqual(_record.DebitAmount, 0);
        }

        [Test]
        public void Test_ExcelMapper_Assigning_Mapping_with_TranceCode_Position()
        {
            _paymentLiner.ExcelReader.Skip(3);
            var excelMap =
                _paymentLiner._fs.FileDetail.FileMappings.Where(
                    x => x.ValueType == ColloSysEnums.FileMappingValueType.ExcelValue).ToList();
            _paymentLiner.ObjRecord.ExcelMapper(_record, excelMap);

            //Assert
            Assert.AreEqual(_record.TransAmount, 5000);
        }

        [Test]
        public void Test_Defaultmapper_Assigning_ValidMapping_Check_BilStatus()
        {
            _paymentLiner.ExcelReader.Skip(3);
            _paymentLiner.ObjRecord.CreateRecord(_paymentLiner._fs.FileDetail.FileMappings, out _record);

            //Assert
            Assert.AreEqual(_record.BillStatus, ColloSysEnums.BillStatus.Unbilled);
        }

        [Test]
        public void Test_ComputtedMapper_Assigning_Valid_Mapping()
        {
            //Act
            _paymentLiner.ExcelReader.Skip(3);
            bool isComputtedSetter = _paymentLiner.ObjRecord.ComputedSetter(_record);

            //Assert
            Assert.AreEqual(isComputtedSetter, true);

        }

        [Test]
        public void Test_CheckBasicField_Assigning_Valid_AccounNo()
        {
            //Act
            _paymentLiner.ExcelReader.Skip(3);
            bool isValidBasicField = _paymentLiner.ObjRecord.CheckBasicField();

            //Assert
            Assert.AreEqual(isValidBasicField, true);

        }

        [Test]
        public void Test_CheckBasicField_Assignsing_InValid_AccounNo()
        {
            //Act
            _paymentLiner.ExcelReader.Skip(1);
            bool isValidBasicField = _paymentLiner.ObjRecord.CheckBasicField();

            //Assert
            Assert.AreEqual(isValidBasicField, false);

        }

        [Test]
        public void Test_CheckBasicField_Assigning_InValidAccountNo_Check_IgnoreCount()
        {
            //Act
            _paymentLiner.ExcelReader.Skip(1);
            _paymentLiner.ObjRecord.CheckBasicField();


            //Assert
            Assert.AreEqual(_paymentLiner.Counter.IgnoreRecord, 1);
        }

        #endregion
        


        


       
    }
}
