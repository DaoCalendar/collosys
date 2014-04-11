using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.FileReader;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.ExcelReader.FileReader
{
    public class FileReader<T> : IFileReader<T> where T : class,new()
    {
        #region:: Members ::

        private readonly CreateRecords<T> _objRecord;
        public  List<T> RecordList; 
        public FileReader()
        {
            _objRecord = new CreateRecords<T>();
        }
        #endregion

        public void ReadAndSaveBatch(T obj, IExcelReader excelReader,IList<FileMapping> mappings,ICounter counter,uint batchSize)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            for (var i = excelReader.CurrentRow; i < excelReader.TotalRows && (!excelReader.EndOfFile()); i = i + batchSize)
            {
              RecordList = new List<T>();
                for (var j = 0; j < batchSize && (!excelReader.EndOfFile()); j++)
                {
                    obj = new T();
                    _objRecord.CreateRecord(obj, excelReader, mappings, counter);
                    RecordList.Add(obj);
                    excelReader.NextRow();
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
