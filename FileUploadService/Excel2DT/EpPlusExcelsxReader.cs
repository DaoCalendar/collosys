#region references

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.FileUploadService.Excel2DT
{
    public static class EpPlusExcelsxReader
    {
        public static DataTable GenerateDataTable(FileScheduler scheduler, FileInfo fileInfo)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(fileInfo.FullName))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                var tbl = new DataTable();
                var fileColumns = scheduler.FileDetail.FileColumns.OrderBy(c => c.Position);
                foreach (var fileColumn in fileColumns)
                {
                    tbl.Columns.Add(fileColumn.TempColumnName, typeof(string));
                }

                for (var rowNum = 1; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    var row = tbl.NewRow();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                    tbl.Rows.Add(row);
                }
                return tbl;
            }
        }

        public static DataTable ReadExcelData<T>(T obj, FileInfo fileInfo, uint dataFromRow = 1) where T : new()
        {
            if (fileInfo == null || !fileInfo.Exists || fileInfo.Length <= 0)
            {
                return null;
            }

            return ReadExcelData(obj, File.OpenRead(fileInfo.FullName), dataFromRow);
        }

        public static DataTable ReadExcelData(Type type, Stream fileStream, uint dataFromRow = 1)
        {
            if (fileStream == null || fileStream.Length == 0 || !fileStream.CanRead)
            {
                return null;
            }

            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = fileStream)
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                var tbl = new DataTable();
                var fileColumns = type.GetProperties();
                foreach (var fileColumn in fileColumns)
                {
                    tbl.Columns.Add(fileColumn.Name, typeof(string));
                }

                for (var rowNum = (int)dataFromRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    var row = tbl.NewRow();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                    tbl.Rows.Add(row);
                }
                return tbl;
            }
        }

        public static DataTable ReadExcelData<T>(T obj, Stream fileStream, uint dataFromRow = 1) where T : new()
        {
            if (fileStream == null || fileStream.Length == 0 || !fileStream.CanRead)
            {
                return null;
            }

            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = fileStream)
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                var tbl = new DataTable();
                var fileColumns = obj.GetType().GetProperties(BindingFlags.Public);
                foreach (var fileColumn in fileColumns)
                {
                    tbl.Columns.Add(fileColumn.Name, typeof(string));
                }

                for (var rowNum = (int)dataFromRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    var row = tbl.NewRow();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                    tbl.Rows.Add(row);
                }
                return tbl;
            }
        }
    }
}
