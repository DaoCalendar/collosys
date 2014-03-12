#region references

using System.IO;
using Excel;
using DataTable = System.Data.DataTable;

#endregion


namespace ColloSys.FileUploadService.Excel2DT
{
    public static class ExcelNugetExcelReader
    {
        public static DataTable GenerateDataTable(FileInfo fileInfo)
        {
            var stream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader;
            switch (fileInfo.Extension.ToLower())
            {
                case ".xls":
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    break;
                case ".xlsx":
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    break;
                default:
                    return null;
            }

            excelReader.IsFirstRowAsColumnNames = false;
            return excelReader.AsDataSet().Tables[0];
        }
    }
}
