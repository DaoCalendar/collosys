using System;
using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.FileUploader.ExcelReader.FileReader;
using ColloSys.FileUploader.ExcelReader.RecordSetter;
using FileUploader.ExcelReader;
using NUnit.Framework;
using ReflectionExtension.ExcelReader;

namespace ReflectionExtension.Tests.ExcelReader.Test
{
    class FileReaderAndSetToListTest : SetUpAssemblies 
    {
        
        private ICounter _counter;
        private FileReader<Payment> _record;
        private IExcelReader _excelReader;
        private IFileReader<Payment> _fileReader;
        private ExcelReaderHelper _excelReaderHelper;
        private MappingInfo _mapping;
        private IList<FileMapping> _mappingList;
        private Payment _obj;

        [SetUp]
        public void Init()
        {
            _record=new FileReader<Payment>();
            _obj = new Payment();
            _excelReaderHelper = new ExcelReaderHelper();
            _excelReader = _excelReaderHelper.GetInstance(FileInfo);
            _mapping = new MappingInfo();
            _counter = new ExcelRecordCounter();
            _fileReader = new FileReader<Payment>();
            _mappingList = SessionManager.GetCurrentSession().QueryOver<FileMapping>().List<FileMapping>();
        }

        [Test]
        public void Test_ReadAndSaveBatch()
        {
            var data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().Where(d => d.FileDetail.Id == Guid.Parse("A42EF611-808D-4CC2-9F6F-D15069664D4C")).List<FileMapping>();
            _excelReader.Skip(2);
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data,_counter);
        }
        [Test]
        public void Test_CreateRecord_ErrorRecordCount()
        {
            var data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().Where(d => d.FileDetail.Id == Guid.Parse("A42EF611-808D-4CC2-9F6F-D15069664D4C")).List<FileMapping>();
            _excelReader.Skip(2);
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data,_counter);
            Assert.AreEqual(_counter.ErrorRecords, 1);
        }
        [Test]
        public void Test_CreateRecord_ValidRecordCount()
        {
            var data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().Where(d => d.FileDetail.Id == Guid.Parse("A42EF611-808D-4CC2-9F6F-D15069664D4C")).List<FileMapping>();
            _excelReader.Skip(2);
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data,_counter);
            Assert.AreEqual(_counter.ValidRecords, 1);
        }

        [Test]
        public void Test_CreateRecord_IgnoreRecordCount()
        {
            var data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().Where(d => d.FileDetail.Id == Guid.Parse("A42EF611-808D-4CC2-9F6F-D15069664D4C")).List<FileMapping>();
            _excelReader.Skip(2);
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data,_counter);
            Assert.AreEqual(_counter.IgnoreRecord, 1);
        }

        [Test]
        public void Test_CreateRecord_InsertRecordCount()
        {
            var data = SessionManager.GetCurrentSession().QueryOver<FileMapping>().Where(d => d.FileDetail.Id == Guid.Parse("A42EF611-808D-4CC2-9F6F-D15069664D4C")).List<FileMapping>();
            _excelReader.Skip(2);
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data,_counter);
            Assert.AreEqual(_counter.InsertRecord, 14);
        }
    }
}
