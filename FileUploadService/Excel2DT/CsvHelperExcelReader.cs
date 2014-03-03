#region references

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploadService.Interfaces;
using CsvHelper;
using NLog;

#endregion


namespace ColloSys.FileUploadService.Excel2DT
{
    public static class CsvHelperExcelReader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static DataTable GenerateDataTable(FileScheduler fileScheduler, FileInfo fileInfo, string delimiter, IRowCounter counter)
        {
            var dt = new DataTable();
            foreach (var fileColumn in fileScheduler.FileDetail.FileColumns.OrderBy(c => c.Position).Where(fileColumn => fileColumn.Position != 0))
            {
                dt.Columns.Add(fileColumn.TempColumnName);
            }

            System.IO.TextReader readFile = new StreamReader(fileInfo.FullName);
            var csv = new CsvReader(readFile);
            csv.Configuration.Delimiter = delimiter;
            csv.Configuration.HasHeaderRecord = false;
            ulong rowNo = 0;

            while (csv.Read())
            {
                if (counter.GetTotalRecordCount() < fileScheduler.FileDetail.SkipLine)
                {
                    rowNo++;
                    counter.AddIgnoredRecord(rowNo);
                    continue;
                }
                if (counter.GetTotalRecordCount() == fileScheduler.FileDetail.SkipLine)
                {
                    if (dt.Columns.Count > csv.CurrentRecord.Length)
                    {
                        Logger.Error(string.Format("Expected Columns({1}) and File Columns({0}) does not match", csv.CurrentRecord.Length, dt.Columns.Count));

                        IList<string> columnname = new List<string>();
                        for (int i = 0; i < csv.CurrentRecord.Length; i++)
                        {
                            columnname.Add(dt.Columns[i].ColumnName + ":" + csv.CurrentRecord.GetValue(i));
                        }

                        var names = string.Join(", ", columnname);
                        Logger.Error(string.Format("Column names are : {0}", names));

                        throw new Exception("Expected Columns and File Columns does not match");
                    }

                    rowNo++;
                    counter.AddIgnoredRecord(rowNo);
                    continue;
                }


                var dataRow = dt.NewRow();
                // var length = (dt.Columns.Count < csv.FieldHeaders.Count()) ? dt.Columns.Count : csv.FieldHeaders.Count();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dataRow[i] = (i < csv.CurrentRecord.Length) ? csv.GetField(i) : string.Empty;
                }
                dt.Rows.Add(dataRow);
            }

            return dt;
        }



    }
}
