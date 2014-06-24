using System;
using System.IO;
using ColloSys.FileUploader.RowCounter;
using ColloSys.FileUploaderService.ExcelReader;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace ReflectionExtension.ExcelReader
{
    public class NpOiExcelReader : IExcelReader
    {
        public uint TotalRows { get; private set; }
        public uint TotalColumns { get; private set; }
        public uint CurrentRow { get; private set; }
        private readonly ISheet _currentWorkSheet;
        private ICounter _counter;
       
        public NpOiExcelReader(FileStream fileStreams)
        {
            HSSFWorkbook workBook;
            var fileStream = new FileStream(fileStreams.Name, FileMode.Open, FileAccess.Read);
            workBook = new HSSFWorkbook(fileStream);
            _currentWorkSheet = workBook.GetSheetAt(0);

            TotalRows = (uint)_currentWorkSheet.LastRowNum + 1;
            TotalColumns = (uint)_currentWorkSheet.GetRow(0).LastCellNum;
        }

        public NpOiExcelReader(FileInfo fileInfo,ICounter counter   )
        {
            HSSFWorkbook workBook;
            // var fileStream = new FileStream(fileInfo.Name, FileMode.Open, FileAccess.Read);
            {
                workBook = new HSSFWorkbook(fileInfo.OpenRead());

            }
            _currentWorkSheet = workBook.GetSheetAt(0);
            TotalRows = (uint)_currentWorkSheet.LastRowNum + 1;
            TotalColumns = (uint)_currentWorkSheet.GetRow(2).LastCellNum;
            _counter = counter;
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
            try
            {
                CurrentRow = _counter.CurrentRow;
                var cell = _currentWorkSheet.GetRow((int)CurrentRow).GetCell((int)pos - 1);
                return cell != null ? cell.GetValue(cell.CellType) : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
                
            }
           
        }
        public string GetValue(uint rowPos, uint pos)
        {
            var cell = _currentWorkSheet.GetRow((int)rowPos ).GetCell((int)pos - 1);
            return cell.GetValue(cell.CellType);
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
