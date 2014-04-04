using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.ExcelReader.RecordSetter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.ExcelReader.FileReader
{
    public class FileReader<T> : IFileReader<T> where T : class,new()
    {
        #region:: Members ::
        public static List<T> _objList = new List<T>();
        public  SetExcelRecord<T> ObjRecord = new SetExcelRecord<T>();
        public ICounter Counter;
        public uint BatchSize { get; private set; }

        public FileReader()
        {
            Counter = new ExcelRecordCounter();
        }

        #endregion
        public void ReadAndSaveBatch(T obj, IExcelReader excelReader, IList<FileMapping> mappings,ICounter counter)
        {
            if (obj == null) throw new ArgumentNullException(" ");
            for (var i = excelReader.CurrentRow; i < excelReader.TotalRows; i++)
            {
                obj = new T();
                ObjRecord.CreateRecord(obj, excelReader, mappings, counter);
                _objList.Add(obj);
                excelReader.NextRow();
            }
        }
    }
}
