using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.ExcelReaders.ExcelReader;
using ColloSys.FileUploader.ExcelReaders.FileReader;
using NHibernate.Hql.Ast.ANTLR;
using NSubstitute;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;

namespace ReflectionExtension.Tests.ExcelReader.Test.AliasWiseReader.Test
{
    [TestFixture]
    class RlsPaymentLinerRecordCreatorTest : SetUpAssemblies
    {
        #region :: constructor::

        private IDbLayer _dbLayer = NSubstitute.Substitute.For<IDbLayer>();
        private IExcelReader _reader;
        private ICounter _counter;
        private Payment _objPayment;
        private RlsPaymentLinerRecordCreator _recordCreator;
        private IList<string> EpaymentExcludeCode;
        private QueryTest _test;
        private RlsPaymentFileReader _obj;
            List<string> strList = new List<string>();
        #endregion
        [SetUp]
        public void Init()
        {

            strList.Add("204@PARTIAL REPAYMENT - REVERSAL");
            strList.Add("PARTIAL REPAYMENT - REVERSAL");
            strList.Add("204");
            _dbLayer.GetValuesFromKey(ColloSysEnums.Activities.FileUploader, "EPaymentExcludeCode").Returns(strList);
            _objPayment = new Payment();
            _reader = new NpOiExcelReader(FileInfo);
            _counter = new ExcelRecordCounter();
            _test=new QueryTest();
            _obj = new RlsPaymentFileReader();
            _recordCreator = new RlsPaymentLinerRecordCreator(EpaymentExcludeCode);
        }

        [Test]
        public void Test_ComputedField_FileDate_Assigning_ValidRow()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _reader.Skip(3);
            _test.GetValuesFromKey();
            _recordCreator.ComputedSetter(_objPayment, _reader, _counter);
            Assert.AreEqual(_objPayment.IsExcluded, true);
        }
        [Test]
        public void Test_ComputedField_FileDate_Assigning_InValidRow()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _reader.Skip(2);
            _recordCreator.CreateRecord(_objPayment, _reader, data, _counter);
            Assert.AreNotEqual(_objPayment.FileDate, Convert.ToDateTime("10/10/2014"));
        }

        [Test]
        public void Test_ComputedField_Exclude_Assigning_Invalid_TransCode()
        {
            var data = ExcelRecordSetTest.GetFileMappings("A42EF611-808D-4CC2-9F6F-D15069664D4C");
            _reader.Skip(4);
            _recordCreator.CreateRecord(_objPayment, _reader, data, _counter);
            Assert.AreEqual(_objPayment.IsExcluded, true);
        }

        [Test]
        public void test()
        {
           IList<string> lst= _dbLayer.GetValuesFromKey(ColloSysEnums.Activities.FileUploader, "EPaymentExcludeCode");
            Assert.AreEqual(lst.Count,3);
        }
    }
}
