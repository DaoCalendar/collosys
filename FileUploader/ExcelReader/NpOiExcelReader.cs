using System.IO;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;

namespace FileUploader.ExcelReader
{
    public class NpOiExcelReader : IExcelReader
    {
        public uint TotalRows { get; private set; }
        public uint TotalColumns { get; private set; }
        public uint CurrentRow { get; private set; }
        private readonly ISheet _currentWorkSheet;

        public NpOiExcelReader(FileStream fileStreams)
        {
            HSSFWorkbook workBook;
            var fileStream = new FileStream(fileStreams.Name, FileMode.Open, FileAccess.Read);
             workBook = new HSSFWorkbook(fileStream);
            _currentWorkSheet = workBook.GetSheetAt(0);
            TotalRows = (uint)_currentWorkSheet.LastRowNum + 1;
            TotalColumns = (uint)_currentWorkSheet.GetRow(0).LastCellNum;
        }

        public NpOiExcelReader(FileInfo fileInfo)
        {
            HSSFWorkbook workBook;
           var fileStream = new FileStream(fileInfo.Name, FileMode.Open, FileAccess.Read);
            {
             workBook = new HSSFWorkbook(fileStream);
                
            }
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
