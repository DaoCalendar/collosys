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
        private IFileReader<RLiner> _RLiner;
        RLiner _record = new RLiner();
        [SetUp]
        public void Init()
        {
            var mappingData = new FileDataProvider();
            var fileScheduler = mappingData.GetUploadedFile(ColloSysEnums.FileAliasName.R_LINER_BFS_LOAN);
            _RLiner = new RLinerBfsFR(fileScheduler);

        }

        [Test]
        public void Test_CreateRecord_Assigning_Valid_Mapping()
        {
            //Act
            _RLiner.ObjRecord.CreateRecord(_RLiner._fs.FileDetail.FileMappings,out _record);

            //Assert
            Assert.AreEqual(_record.AccountNo,132);
        }
    }
}
