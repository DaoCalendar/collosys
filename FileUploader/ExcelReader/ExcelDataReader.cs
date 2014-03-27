using System;
using System.IO;
using Excel;

namespace ReflectionExtension.ExcelReader
{
    public class ExcelDataReader : IExcelReader
    {
        public uint TotalRows { get; private set; }
        public uint TotalColumns { get; private set; }
        public uint CurrentRow { get; private set; }
        private readonly IExcelDataReader _excelReader;

        public ExcelDataReader(FileStream fileStream)
        {
            var file = new FileStream(fileStream.Name, FileMode.Open, FileAccess.Read);
            string extention = Path.GetExtension(file.Name);
            _excelReader = extention != null && extention.Contains(".xlsx")
                      ? ExcelReaderFactory.CreateOpenXmlReader(file)
                      : ExcelReaderFactory.CreateBinaryReader(file);
            var dataset = _excelReader.AsDataSet();
            TotalRows = (uint)dataset.Tables[0].Rows.Count;
            TotalColumns = (uint)dataset.Tables[0].Columns.Count;
            _excelReader.Read();
            CurrentRow = 1;
        }

        public ExcelDataReader(FileInfo fileInfo)
        {
            //var file = new FileStream(fileInfo.Name, FileMode.Open, FileAccess.Read);
            var extention = Path.GetExtension(fileInfo.Name);
            _excelReader = extention.Contains(".xlsx")
                      ? ExcelReaderFactory.CreateOpenXmlReader(fileInfo.OpenRead())
                      : ExcelReaderFactory.CreateBinaryReader(fileInfo.OpenRead());
            var dataset = _excelReader.AsDataSet();
            TotalRows = (uint)dataset.Tables[0].Rows.Count;
            TotalColumns = (uint)dataset.Tables[0].Columns.Count;
            _excelReader.Read();
            CurrentRow = 1;

        }

        public void NextRow()
        {
            if (!EndOfFile())
            {
                _excelReader.Read();
                CurrentRow = (uint)_excelReader.Depth;
            }

        }

        public bool EndOfFile()
        {
            return _excelReader.Depth > TotalRows;
        }

        public string GetValue(uint pos)
        {
            if (!_excelReader.IsValid) return string.Empty;
            return _excelReader.GetValue((int)pos - 1).ToString();
        }

        public string GetValue(uint rowPos, uint colPos)
        {
            throw new NotImplementedException();
        }

        public void Skip(uint count)
        {
            for (int i = 0; i < count; i++)
            {
                NextRow();
            }
        }
    }
}
