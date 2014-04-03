using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.ExcelReader.RecordSetter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.ExcelReader.FileReader
{
    public class FileReader<T> : IFileReader<T> where T : class,new()
    {
        private readonly List<T> _objList = new List<T>();

        //public FileReader()
        //{
        //    _objList = new List<T>();
        //}

        readonly SetExcelRecord<T> _objRecord = new SetExcelRecord<T>();
        public uint BatchSize { get; private set; }
        public void ReadAndSaveBatch(T obj, IExcelReader excelReader, IList<FileMapping> mappings)
        {
            for (var i = excelReader.CurrentRow; i < excelReader.TotalRows; i++)
            {
                obj = new T();
                _objRecord.CreateRecord(obj, excelReader, mappings);
                _objList.Add(obj);
                excelReader.NextRow();
            }

        }

    }
}
