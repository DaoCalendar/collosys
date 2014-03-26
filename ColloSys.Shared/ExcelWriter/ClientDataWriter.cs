#region references

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.Shared.SharedUtils;
using OfficeOpenXml;
using OfficeOpenXml.Style;

#endregion

namespace ColloSys.Shared.ExcelWriter
{
    public static class ClientDataWriter
    {
        #region convert to T and then call generate excel

        public static void ListToExcel(IList listdata, FileInfo file, IList<ColumnPositionInfo> includedProps = null)
        {
            if (listdata.Count == 0)
            {
                ListToExcelGlobal(null, file, (CustomerInfo)Activator.CreateInstance(typeof(CustomerInfo)), includedProps);
            }
            else
            {
                ListToExcelGeneric(listdata, file, includedProps);
            }
        }

        public static void ListToExcel<T>(IList<T> listdata, FileInfo file, IList<ColumnPositionInfo> includedProps = null)
        {
            if (listdata.Count == 0)
            {
                ListToExcelGlobal(null, file, (CustomerInfo)Activator.CreateInstance(typeof(CustomerInfo)), includedProps);
            }
            else
            {
                ListToExcelGlobal(listdata, file, listdata.First(), includedProps);
            }
        }

        #endregion

        #region excel writer

        private static void ListToExcelGeneric(IList listdata, FileInfo file, IList<ColumnPositionInfo> includedProps)
        {
            var enumerator = listdata.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                if (!file.Exists) file.Create();
                return;
            }
            dynamic firstItem = enumerator.Current;
            dynamic typedEnumerable = ConvertTyped(listdata, firstItem);
            ListToExcelGlobal(typedEnumerable, file, firstItem, includedProps);
        }

        // ReSharper disable UnusedParameter.Local
        private static void ListToExcelGlobal<TEntity>(IEnumerable<TEntity> listdata, FileInfo file,
                                                        TEntity oEntity, IList<ColumnPositionInfo> includedProps)
        {
            using (var pck = new ExcelPackage())
            {
                if (file.Exists)
                {
                    return;
                }

                //Create the worksheet
                var ws = pck.Workbook.Worksheets.Add("Result");
                if (listdata == null)
                {
                    ws.Cells[1, 1].Value = "No Data";
                    pck.SaveAs(file);
                    return;
                }

                // get properties to write
                var entityAllPropsList = typeof(TEntity).GetProperties();
                if (includedProps == null)
                {
                    uint i = 0;
                    includedProps = (from d in entityAllPropsList
                                     select new ColumnPositionInfo()
                                         {
                                             FieldName = d.Name,
                                             DisplayName = d.Name,
                                             Position = (++i),
                                             WriteInExcel = true,
                                             UseFieldNameForDisplay = false,
                                             IsFreezed = false
                                         }).ToList();
                }

                foreach (var column in includedProps)
                {
                    var prop = entityAllPropsList.SingleOrDefault(x => x.Name == column.FieldName.Split('.').FirstOrDefault());
                    if (prop == null) column.WriteInExcel = false;
                    else column.PropertyInfo = prop;
                }

                //get our column headings
                var propsToWrite = includedProps.Where(x => x.WriteInExcel && x.PropertyInfo != null).ToList();
                var propsToWriteCount = propsToWrite.Count(x => x.WriteInExcel);
                var writeTwoHeaders = (propsToWrite.Count(x => x.UseFieldNameForDisplay) > 0) &&
                    (propsToWrite.Count(x => !string.IsNullOrWhiteSpace(x.FieldName)) > 0);
                for (var i = 0; i < propsToWriteCount; i++)
                {
                    if (writeTwoHeaders)
                    {
                        var info = propsToWrite.ElementAt(i);
                        var line2Header = info.UseFieldNameForDisplay ? info.FieldName : info.DisplayName;
                        var line1Header = info.UseFieldNameForDisplay ? info.DisplayName : info.FieldName;
                        line1Header = line1Header == line2Header ? string.Empty : line1Header;
                        ws.Cells[1, i + 1].Value = line1Header;
                        ws.Cells[2, i + 1].Value = line2Header;
                    }
                    else
                    {
                        ws.Cells[1, i + 1].Value = propsToWrite.ElementAt(i).DisplayName;
                    }
                }

                //freeze/hide headers
                var rows2Freeze = (writeTwoHeaders ? 2 : 1) + 1;
                var column2Freeze = 1;
                foreach (var info in propsToWrite)
                {
                    if (info.IsFreezed) column2Freeze++;
                    else break;
                }
                ws.View.FreezePanes(rows2Freeze, column2Freeze);
                if (writeTwoHeaders) ws.Row(1).Hidden = true;

                //format the header
                var range = writeTwoHeaders ? string.Format("A2:{0}2", GetExcelColumnName(propsToWriteCount))
                    : string.Format("A1:{0}1", GetExcelColumnName(propsToWriteCount));
                using (var rng = ws.Cells[range])
                {
                    rng.AutoFilter = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.BurlyWood);
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.Black);
                }

                //set the column types
                var propertyCounter = 0;
                foreach (var prop in propsToWrite)
                {
                    var type = ReflectionUtil.GetPropertyType(typeof (TEntity), prop.FieldName);
                    var filedName = prop.FieldName.Split('.').Last();
                    var jstype = Utilities.GetBaseValueType(filedName, type);
                    switch (jstype)
                    {
                        case ColloSysEnums.BasicValueTypes.Date:
                            ws.Column(++propertyCounter).Style.Numberformat.Format = "yyyy-mm-dd";
                            break;
                        case ColloSysEnums.BasicValueTypes.DateTime:
                            ws.Column(++propertyCounter).Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
                            break;
                        default:
                            ws.Column(++propertyCounter).Style.Numberformat.Format = SetNumberFormat(prop.PropertyInfo.PropertyType);
                            break;
                    }
                }

                //populate our Data
                var excelRowCounter = rows2Freeze;
                foreach (var entity in listdata)
                {
                    propertyCounter = 1;
                    foreach (var prop in propsToWrite)
                    {
                        ws.Cells[excelRowCounter, propertyCounter].Value
                            = ReflectionUtil.GetPropertyValue(entity, prop.FieldName);
                        propertyCounter = propertyCounter + 1;
                    }
                    excelRowCounter = excelRowCounter + 1;
                }

                pck.SaveAs(file);
            }
        }

        // ReSharper restore UnusedParameter.Local
        #endregion

        #region convert IEnumberable to IEnumerable<T>

        // ReSharper disable UnusedParameter.Global
        public static IEnumerable<T> ConvertTyped<T>(IEnumerable source, T firstItem)
        // ReSharper restore UnusedParameter.Global
        {
            // Note: firstItem parameter is unused and is just for resolving type of T
            return from object item in source select (T)item;
        }

        #endregion

        #region convert int to excel column name e.g. 1 to A

        private static string GetExcelColumnName(int columnNumber)
        {
            var dividend = columnNumber;
            var columnName = String.Empty;

            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = ((dividend - modulo) / 26);
            }

            return columnName;
        }

        private static string SetNumberFormat(Type type)
        {
            string result = "@";

            if (type == typeof(string))
                result = "@";
            else if (type == typeof(sbyte))
                result = "##0";
            else if (type == typeof(byte))
                result = "##0";
            else if (type == typeof(bool))
                result = "0";
            else if (type == typeof(short))
                result = "####0";
            else if (type == typeof(ushort))
                result = "####0";
            else if (type == typeof(int))
                result = "#,##0";
            else if (type == typeof(uint))
                result = "#,##0";
            else if (type == typeof(long))
                result = "#,##0";
            else if (type == typeof(ulong))
                result = "#,##0";
            else if (type == typeof(float))
                result = "#,##0.#######";
            else if (type == typeof(double))
                result = "#,##0.################";
            else if (type == typeof(decimal))
                result = "#,##0.00##########################";
            else if (type == typeof(DateTime))
                result = "yyyy-mm-dd";

            return result;
        }

        #endregion
    }
}
