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
    public static class ExcelWriter
    {
        #region convert to T and then call generate excel

        public static void ListToExcel(IList listdata, FileInfo file, IList<string> excludedProps = null)
        {
            if (listdata.Count == 0)
            {
                ListToExcelGlobal(null, file, (CustomerInfo)Activator.CreateInstance(typeof(CustomerInfo)), excludedProps);
            }
            else
            {
                ListToExcelGeneric(listdata, file, excludedProps);
            }
        }

        public static void ListToExcel<T>(IList<T> listdata, FileInfo file, IList<string> excludedProps = null)
        {
            if (listdata.Count == 0)
            {
                ListToExcelGlobal(null, file, (CustomerInfo)Activator.CreateInstance(typeof(CustomerInfo)), excludedProps);
            }
            else
            {
                ListToExcelGlobal(listdata, file, listdata.First(), excludedProps);
            }
        }

        #endregion

        #region excel writer

        private static void ListToExcelGeneric(IList listdata, FileInfo file, IList<string> excludedProps)
        {
            var enumerator = listdata.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                if (!file.Exists) file.Create();
                return;
            }
            dynamic firstItem = enumerator.Current;
            dynamic typedEnumerable = ConvertTyped(listdata, firstItem);
            ListToExcelGlobal(typedEnumerable, file, firstItem, excludedProps);
        }

        // ReSharper disable UnusedParameter.Local
        private static void ListToExcelGlobal<TEntity>(IEnumerable<TEntity> listdata, FileInfo file,
                                                        TEntity oEntity, IList<string> excludedProps)
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

                //get our column headings
                if (excludedProps == null) excludedProps = new List<string>();
                var propertylist = typeof(TEntity).GetProperties().Where(x => !excludedProps.Contains(x.Name)).ToList();
                for (var i = 0; i < propertylist.Count; i++)
                {
                    //var nameparts = propertylist.ElementAt(i).Name.Split('.');
                    //ws.Cells[1, i + 1].Value = nameparts[nameparts.Count() - 1];
                    ws.Cells[1, i + 1].Value = propertylist.ElementAt(i).Name;
                }
                ws.View.FreezePanes(2, 1);

                //format the header
                var range = string.Format("A1:{0}1", GetExcelColumnName(propertylist.Count()));
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
                foreach (var prop in propertylist)
                {
                    var jstype = Utilities.GetBaseValueType(prop.Name, prop.PropertyType);
                    switch (jstype)
                    {
                        case ColloSysEnums.BasicValueTypes.Date:
                            ws.Column(++propertyCounter).Style.Numberformat.Format = "yyyy-mm-dd";
                            break;
                        case ColloSysEnums.BasicValueTypes.DateTime:
                            ws.Column(++propertyCounter).Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
                            break;
                        default:
                            ws.Column(++propertyCounter).Style.Numberformat.Format = SetNumberFormat(prop.PropertyType);
                            break;
                    }
                }

                //populate our Data
                var excelRowCounter = 2;
                foreach (var entity in listdata)
                {
                    propertyCounter = 1;
                    foreach (var propertyInfo in propertylist)
                    {
                        ws.Cells[excelRowCounter, propertyCounter].Value = propertyInfo.GetValue(entity, null);
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
