#region references

using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

#endregion

namespace ColloSys.FileUploadService.Excel2DT
{
    public static class NPOIExcelReader
    {
        public static DataTable GenerateDataTable(FileScheduler scheduler, FileInfo fileName)
        {
            var dt = new DataTable();
            HSSFWorkbook hssfworkbook;
            using (var file = new FileStream(fileName.FullName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            var sheet = hssfworkbook.GetSheetAt(0);
            if (sheet == null)
            {
                return dt;
            }

            var rows = sheet.GetRowEnumerator();

            while (rows.MoveNext())
            {
                try
                {
                    IRow row = (HSSFRow)rows.Current;

                    if (dt.Columns.Count == 0)
                    {
                        var fileColumns = scheduler.FileDetail.FileColumns.OrderBy(c => c.Position);
                        foreach (var fileColumn in fileColumns)
                        {
                            dt.Columns.Add(fileColumn.TempColumnName, typeof(string));
                        }

                        continue;
                    }

                    var dr = dt.NewRow();
                    var loopCount = (dt.Columns.Count < row.LastCellNum) ? dt.Columns.Count : row.LastCellNum;
                    //for (int i = row.FirstCellNum; i < row.LastCellNum; i++)
                    for (int i = row.FirstCellNum; i < loopCount; i++)
                    {
                        var cell = row.GetCell(i);
                        
                        if (cell == null)
                        {
                            dr[i] = null;
                        }
                        else
                        {
                            dr[i] = cell.GetValue(cell.CellType);
                        }
                    }
                    dt.Rows.Add(dr);
                }
                catch (Exception)
                {
                    return dt;
                }
            }

            return dt;
        }

        private static string GetValue(this ICell cell, CellType cellType)
        {
            string value;

            switch (cellType)
            {
                case CellType.NUMERIC:
                    value = cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                    break;
                case CellType.STRING:
                    value = cell.RichStringCellValue.String;
                    break;
                case CellType.FORMULA:
                    value = cell.GetValue(cell.CachedFormulaResultType);
                    break;
                case CellType.BOOLEAN:
                    value = cell.BooleanCellValue.ToString(CultureInfo.InvariantCulture);
                    break;
                default:
                    value = string.Empty;
                    break;
            }

            return value;
        }

        public static DataTable GenerateCLinerDataTable(FileScheduler fileScheduler, FileInfo fileName)
        {
            var dt = new DataTable();
            HSSFWorkbook hssfworkbook;
            using (var file = new FileStream(fileName.FullName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            var sheet = hssfworkbook.GetSheetAt(0);
            if (sheet == null)
            {
                return dt;
            }

            var rows = sheet.GetRowEnumerator();
            int rowNo = 1;
            while (rows.MoveNext())
            {
                try
                {
                    IRow row = (HSSFRow)rows.Current;

                    if (dt.Columns.Count == 0)
                    {
                        dt.Columns.Add("CLinerId", typeof(Guid));
                        dt.Columns.Add("Version");
                        dt.Columns.Add("CreatedBy");
                        dt.Columns.Add("CreatedOn");
                        dt.Columns.Add("CreateAction");

                        for (int i = row.FirstCellNum; i < row.LastCellNum; i++)
                        {
                            var cell = row.GetCell(i);
                            dt.Columns.Add(cell.GetValue(cell.CellType));
                        }

                        dt.Columns.Add("CustStatus");
                        dt.Columns.Add("FileDate");
                        dt.Columns.Add("FileRowNo");
                        dt.Columns.Add("IsReferred");
                        dt.Columns.Add("Pincode");
                        dt.Columns.Add("Product");
                        dt.Columns.Add("GPincodeId", typeof(Guid));
                        dt.Columns.Add("AllocStatus");
                        dt.Columns.Add("NoAllocResons");
                        dt.Columns.Add("FileSchedulerId", typeof(Guid));

                        dt.Columns["Unbill"].DefaultValue = 0;
                        dt.Columns["BktAmt"].DefaultValue = 0;
                        continue;
                    }

                    var dr = dt.NewRow();
                    dr["CLinerId"] = Guid.NewGuid();
                    dr["Version"] = 1;
                    dr["CreatedBy"] = "ActualUpload";
                    dr["CreatedOn"] = DateTime.Now;
                    dr["CreateAction"] = "Insert";

                    for (int i = row.FirstCellNum; i < row.LastCellNum; i++)
                    {
                        var cell = row.GetCell(i);

                        var colIndex = i + 5;
                        if (cell == null)
                        {
                            dr[colIndex] = dt.Columns[colIndex].DefaultValue;
                        }
                        else
                        {
                            dr[colIndex] = GetCLinerValue(dt.Columns[colIndex].ColumnName, cell.GetValue(cell.CellType));
                        }
                    }

                    dr["CustStatus"] = "Liner";
                    dr["FileDate"] = fileScheduler.FileDate;
                    dr["FileRowNo"] = rowNo;
                    dr["IsReferred"] = false;
                    dr["Pincode"] = 0;
                    dr["Product"] = ScbEnums.Products.CC;
                    dr["GPincodeId"] = DBNull.Value;
                    dr["AllocStatus"] = ColloSysEnums.AllocStatus.None;
                    dr["NoAllocResons"] = null;
                    dr["FileSchedulerId"] = fileScheduler.Id;

                    dt.Rows.Add(dr);

                    rowNo++;
                }
                catch (Exception)
                {
                    return dt;
                }
            }

            return dt;
        }

        private static string GetCLinerValue(string columnName, string value)
        {
            switch (columnName)
            {
                case "Flag":
                    switch (value)
                    {
                        case "O": return ColloSysEnums.DelqFlag.O.ToString();
                        case "N": return ColloSysEnums.DelqFlag.N.ToString();
                        case "R": return ColloSysEnums.DelqFlag.R.ToString();
                        default: return ColloSysEnums.DelqFlag.Z.ToString();
                    }
                case "Unbill":
                    return value ?? "0";
                case "Bucket":
                    return (value == "X") ? "0" : value;
                case "AcType":
                    switch (value)
                    {
                        case "NORM": return ColloSysEnums.DelqAccountStatus.Norm.ToString();
                        case "PEND": return ColloSysEnums.DelqAccountStatus.PEND.ToString();
                        case "BFL-1": return ColloSysEnums.DelqAccountStatus.BFL1.ToString();
                        case "BFL-2": return ColloSysEnums.DelqAccountStatus.BFL1.ToString();
                        case "BFL-3": return ColloSysEnums.DelqAccountStatus.BFL1.ToString();
                        case "BFL-4": return ColloSysEnums.DelqAccountStatus.BFL1.ToString();
                        case "BFL-5": return ColloSysEnums.DelqAccountStatus.BFL1.ToString();
                        case "BFL-6": return ColloSysEnums.DelqAccountStatus.BFL1.ToString();
                        case "BFL-7": return ColloSysEnums.DelqAccountStatus.BFL1.ToString();
                        default: return ColloSysEnums.DelqAccountStatus.BFL7.ToString();
                    }
                case "LPMNTDT":
                    if (string.IsNullOrWhiteSpace(value))
                        return null;

                    double oADate;
                    if (double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out oADate))
                    {
                        return DateTime.FromOADate(oADate).ToString("o");
                    }

                    DateTime date;
                    return DateTime.TryParseExact(value, "d-MMM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                                                  out date) ? date.ToString("o") : null;
                default:
                    return value;
            }
        }
    }
}
