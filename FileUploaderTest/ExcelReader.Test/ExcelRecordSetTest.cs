using System;
using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.FileUploader.ExcelReader.RecordSetter;
using FileUploader.ExcelReader;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;

namespace ReflectionExtension.Tests.ExcelReader.Test
{
    [TestFixture]
    class ExcelRecordSetTest : SetUpAssemblies
    {
        private IExcelRecord<Payment> _record;
        private ICounter _counter;
        private IExcelReader _excelReader;
        private ExcelReaderHelper _excelReaderHelper;
        private MappingInfo _mapping;
        private IList<FileMapping> _mappingList;
        private DefaultMapping _defaultMapping;
        private Payment _obj;

        [SetUp]
        public void Init()
        {
            _obj = new Payment();
            _record = new SetExcelRecord<Payment>();
            _excelReaderHelper = new ExcelReaderHelper();
            _excelReader = _excelReaderHelper.GetInstance(FileInfo);
            _mapping = new MappingInfo();
            _defaultMapping = new DefaultMapping();
            _counter = new ExcelRecordCounter();
            _mappingList = SessionManager.GetCurrentSession().QueryOver<FileMapping>().List<FileMapping>();
        }



        [Test]
        public void Test_ExcelRecord_ReadAndSet()
        {
            var data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().Where(d => d.ValueType == ColloSysEnums.FileMappingValueType.ExcelValue).Where(d => d.FileDetail.Id == Guid.Parse("A42EF611-808D-4CC2-9F6F-D15069664D4C")).List<FileMapping>();
            _excelReader.Skip(1);
            _record.ExcelMapper(_obj, _excelReader, data);
            Assert.AreEqual(_obj.DebitAmount.ToString(), "11158.36");

        }

        [Test]
        public void Test_Defaultvalue_And_Set()
        {

            var data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().Where(d => d.ValueType == ColloSysEnums.FileMappingValueType.DefaultValue).Where(d => d.FileDetail.Id == Guid.Parse("A42EF611-808D-4CC2-9F6F-D15069664D4C")).List<FileMapping>();
            _record.DefaultMapper(_obj, data);
            Assert.AreEqual(_excelReaderHelper.Property1, "algosys");

        }

        [Test]
        public void Test_Computedvalue_And_Set()
        {
            _excelReader.Skip(1);
            _record.ComputedSetter(_obj, _excelReader);
            Assert.AreEqual(_obj.TransAmount.ToString(), "11158.36");
        }

        [Test]
        public void Test_CreateRecord_TotalCount()
        {
            var data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().Where(d => d.FileDetail.Id == Guid.Parse("A42EF611-808D-4CC2-9F6F-D15069664D4C")).List<FileMapping>();
            _excelReader.Skip(1);
            _record.CreateRecord(_obj, _excelReader, data);
            Assert.AreEqual(_record.TotalCount, 11);
        }

        [Test]
        public void Test_CreateRecord_ValidRecord_Count()
        {
            var data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().Where(d => d.FileDetail.Id == Guid.Parse("A42EF611-808D-4CC2-9F6F-D15069664D4C")).List<FileMapping>();
            _excelReader.Skip(3);
            bool s = _record.CreateRecord(_obj, _excelReader, data);
            Assert.AreEqual(s, true);
        }
        [Test]
        public void Test_CreateRecord_With_InValidRecord_IgnoreCount()
        {
            var data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().Where(d => d.FileDetail.Id == Guid.Parse("A42EF611-808D-4CC2-9F6F-D15069664D4C")).List<FileMapping>();
            _excelReader.Skip(2);
            bool s = _record.CreateRecord(_obj, _excelReader, data);
            Assert.AreEqual(s, true);
        }

        [Test]
        public void Test_UinqExcelMapper_ValidRecord_Count()
        {
            var data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().Where(d => d.ActualColumn == "AccountNo").Where(d => d.FileDetail.Id == Guid.Parse("A42EF611-808D-4CC2-9F6F-D15069664D4C")).List<FileMapping>();
            _excelReader.Skip(1);
            bool s = _record.UniqExcelMapper(_obj, _excelReader, data);
            Assert.AreEqual(s, false);
        }

        [Test]
        public void Test_CreateRecord()
        {
            var data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().Where(d => d.FileDetail.Id == Guid.Parse("A42EF611-808D-4CC2-9F6F-D15069664D4C")).List<FileMapping>();
            _excelReader.Skip(2);
            bool s = _record.CreateRecord(_obj, _excelReader, data);
            Assert.AreEqual(s, false);
        }
    }
}
