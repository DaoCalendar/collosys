using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace FileUploader.ExcelReader
{
    public class EpPlusExcelReader : IExcelReader
    {
        private readonly ExcelWorksheet _worksheet;
        public uint TotalRows { get; private set; }
        public uint TotalColumns { get; private set; }
        public uint CurrentRow { get; private set; }

        public EpPlusExcelReader(FileStream fileStream)
        {
            var file = new FileStream(fileStream.Name, FileMode.Open, FileAccess.Read);
            using (var package = new ExcelPackage(file))
            {
                _worksheet = package.Workbook.Worksheets.First();
            }
           
            TotalRows = (uint)_worksheet.Dimension.End.Row;
            TotalColumns = (uint)_worksheet.Dimension.End.Column;
        }
        public EpPlusExcelReader(FileInfo fileInfo)
        {
            using (var package = new ExcelPackage(fileInfo))
            {
                _worksheet = package.Workbook.Worksheets.First();
            }

            TotalRows = (uint)_worksheet.Dimension.End.Row;
            TotalColumns = (uint)_worksheet.Dimension.End.Column;
        }

        public void NextRow()
        {
            if (EndOfFile() != true)
            {
                CurrentRow++;
            }
            else
            {
                throw new Exception("Current row is out of range");
            }
        }

        public bool EndOfFile()
        {
            return CurrentRow == TotalRows;
        }

        public string GetValue(uint pos)
        {
            var value = _worksheet.Cells[(int) CurrentRow, (int) pos].Value;
            return value.ToString();
        }

        public string GetValue(uint rowPosition, uint columnPosition)
        {
            var value = _worksheet.Cells[(int) rowPosition, (int) columnPosition].Value;
            return value.ToString();
        }

    }
}
