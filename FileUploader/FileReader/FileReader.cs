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
        public uint I { get; private set; }
        public IList<T> List { get; private set; }
        private readonly IExcelReader _excelReader;
        private readonly uint _batchSize;
        private readonly FileScheduler _fs;
       

        protected FileReader(IAliasRecordCreator<T> recordCreator)
        {
             _fs = recordCreator.FileScheduler;
            string path = _fs.FileDirectory  +@"\";

            _excelReader = SharedUtility.GetInstance(new FileInfo(path+_fs.FileName));
            _objRecord = new RecordCreator<T>(recordCreator, _excelReader);
            _batchSize = 100;
        }
        #endregion

        public void ReadAndSaveBatch()
        {
            for (  I = _excelReader.CurrentRow; I < _excelReader.TotalRows && (!_excelReader.EndOfFile()); I = I + _batchSize)
            {
                List = new List<T>();
                for (var j = 0; j < _batchSize && (!_excelReader.EndOfFile()); j++)
                {
                    var obj = new T();
                    var isRecordCreate = _objRecord.CreateRecord(obj, _fs.FileDetail.FileMappings);
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
