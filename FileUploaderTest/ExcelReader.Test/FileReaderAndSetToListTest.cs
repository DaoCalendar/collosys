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
        private IExcelRecord<Payment> _record;
        private ICounter _counter;
        private IExcelReader _excelReader;
        private IFileReader<Payment> _fileReader;
        private ExcelReaderHelper _excelReaderHelper;
        private MappingInfo _mapping;
        private IList<FileMapping> _mappingList;
        private Payment _obj;

        [SetUp]
        public void Init()
        {
            _obj = new Payment();
            _record = new SetExcelRecord<Payment>();
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
            _excelReader.Skip(3);
            _fileReader.ReadAndSaveBatch(_obj, _excelReader, data);
        }
    }
}
