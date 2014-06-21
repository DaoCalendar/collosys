using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.AliasLiner.Rls;
using ColloSys.FileUploaderService.AliasPayment;
using ColloSys.FileUploaderService.FileReader;
using NUnit.Framework;

namespace ReflectionExtension.Tests.AliasLinerTest
{
    [TestFixture]
    class RLinerBfsTest
    {
        private IFileReader<RLiner> _rLiner;
        RLiner _record = new RLiner();
        [SetUp]
        public void Init()
        {
             var date=new DateTime(2013,06,18);
            const string dirPath = "E:/Abhijeet/Collosys2";
            var mappingData = new FileDataProvider(date,dirPath);
            var fileScheduler = mappingData.GetUploadedFile(ColloSysEnums.FileAliasName.R_LINER_BFS_LOAN);
            _rLiner = new RLinerBfsFR(fileScheduler);

        }

        [Test]
        public void Test_CreateRecord_Assigning_Valid_Mapping()
        {
            //Act
            _rLiner.ExcelReader.Skip(3);
            _rLiner.RecordCreatorObj.CreateRecord(_rLiner.FileScheduler.FileDetail.FileMappings,out _record);
            
            //Assert
            Assert.AreEqual(_record.AccountNo,132);
        }
    }
}
