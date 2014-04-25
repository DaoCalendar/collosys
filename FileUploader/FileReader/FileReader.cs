using System;
using System.Collections.Generic;
using System.IO;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.RecordCreator;
using ColloSys.FileUploader.Utilities;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.FileReader
{
    public abstract class FileReader<T> : IFileReader<T> where T : class,new()
    {
        #region:: Members ::

        private readonly IRecord<T> _objRecord;
        public IList<T> List { get; private set; }
        private readonly IExcelReader _excelReader;

        protected FileReader(IAliasRecordCreator<T> recordCreator)
        {
            var fs = recordCreator.FileScheduler;
            _excelReader = SharedUtility.GetInstance(new FileInfo(fs.FileDirectory));
            _objRecord = new RecordCreator<T>(recordCreator, _excelReader);
        }
        #endregion

        public void ReadAndSaveBatch(T obj, IEnumerable<FileMapping> mappings, uint batchSize)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            for (var i = _excelReader.CurrentRow; i < _excelReader.TotalRows && (!_excelReader.EndOfFile()); i = i + batchSize)
            {
                List = new List<T>();
                for (var j = 0; j < batchSize && (!_excelReader.EndOfFile()); j++)
                {
                    obj = new T();
                    var isRecordCreate = _objRecord.CreateRecord(obj, mappings);
                    if (isRecordCreate)
                    {
                        List.Add(obj);
                    }
                    _excelReader.NextRow();
                }
                //var session = SessionManager.GetCurrentSession();
                //using (var transaction=session.BeginTransaction())
                //{
                //    foreach (var record in RecordList)
                //    {
                //        session.Save(record);
                //    }

                //    transaction.Commit();
                //}
            }
        }
    }
}
