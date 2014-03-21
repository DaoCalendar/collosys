using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace FileUploader.ExcelReader
{
    public class NpOiExcelReader : IExcelReader
    {
        public uint TotalRows { get; private set; }
        public uint TotalColumns { get; private set; }
        public uint CurrentRow { get; private set; }
        private readonly ISheet _currentWorkSheet;

        public NpOiExcelReader(FileInfo fileInfo)
        {
            HSSFWorkbook workBook;
            using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
            {
                 workBook = new HSSFWorkbook(fileStream);
            }
           // CurrentRow = 0;
            _currentWorkSheet = workBook.GetSheetAt(0);
            TotalRows = (uint)_currentWorkSheet.LastRowNum + 1;
            TotalColumns = (uint)_currentWorkSheet.GetRow(0).LastCellNum;
        }

        public void NextRow()
        {
            if (!EndOfFile())
            {
                CurrentRow++;
            }
        }

        public bool EndOfFile()
        {
            return CurrentRow == TotalRows;
        }

        public string GetValue(uint pos)
        {
            var cell = _currentWorkSheet.GetRow((int)CurrentRow - 1).GetCell((int)pos - 1);
            return cell.GetValue(cell.CellType);

        }
        public string GetValue(uint rowPos, uint pos)
        {
            var cell = _currentWorkSheet.GetRow((int)rowPos - 1).GetCell((int)pos - 1);
            return cell.GetValue(cell.CellType);
        }
    }
}
