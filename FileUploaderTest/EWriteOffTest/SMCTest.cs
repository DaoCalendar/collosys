using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.AliasPayment;
using ColloSys.FileUploaderService.AliasWriteOff.Ebbs;
using ColloSys.FileUploaderService.FileReader;
using NUnit.Framework;

namespace ReflectionExtension.Tests.EWriteOffTest
{
    public class SMCTest : FileProvider
    {
        #region ctor

        private IFileReader<EWriteoff> _paymentLiner;
        EWriteoff _record = new EWriteoff();

        [SetUp]
        public void Init()
        {
            var fileDate = new DateTime(2013, 08, 17);
            const string fileDirectory = "C:/Users/Ast011/Documents/collosys";
            var mappingData = new FileDataProvider(fileDate,fileDirectory);
            var fileScheduler = mappingData.GetUploadedFile(ColloSysEnums.FileAliasName.E_WRITEOFF_SMC);
            _paymentLiner = new EWriteOffSmcFR(fileScheduler);
        }

        #endregion

        [Test]
        public void Test_CreateRecord_Assigning_ValidMapping_Check_AccNo()
        {
            _paymentLiner.ExcelReader.Skip(3);
           var record= _paymentLiner.RecordCreatorObj.CreateRecord(_paymentLiner.FileScheduler.FileDetail.FileMappings, out _record);
           
            //Assert
            Assert.AreEqual(_record.AccountNo, "12345678902");
        }
    }
}
