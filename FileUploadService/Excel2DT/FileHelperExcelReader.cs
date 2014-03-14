#region references

using System.Data;
using System.IO;
using System.Linq;
using ColloSys.DataLayer.Domain;
using FileHelpers.DataLink;
//using FileHelpers.Dynamic;
using FileHelpers.Dynamic;

#endregion


namespace ColloSys.FileUploadService.Excel2DT
{
    public static class FileHelperExcelReader
    {
        public static DataTable GenerateDataTable(FileScheduler fileScheduler, FileInfo fileInfo)
        {
            var cb = new DelimitedClassBuilder(string.Format("{0}_VerticalBar", fileScheduler.FileDetail.TempTable), "|")
            {
                IgnoreFirstLines = 1,
                IgnoreEmptyLines = true
            };

            var fileColumns = fileScheduler.FileDetail.FileColumns.OrderBy(c => c.Position);
            foreach (var fileColumn in fileColumns)
            {
                cb.AddField(fileColumn.TempColumnName, typeof(string));
            }

            cb.LastField.FieldQuoted = true;
            cb.LastField.QuoteChar = '"';

            var provider = new ExcelStorage(cb.CreateRecordClass())
            {
                StartRow = (int)fileScheduler.FileDetail.SkipLine + 2,
                StartColumn = 1,
                FileName = fileInfo.FullName 
            };

            var dataTable = provider.ExtractRecordsAsDT();

            return dataTable;
        }

    }
}
