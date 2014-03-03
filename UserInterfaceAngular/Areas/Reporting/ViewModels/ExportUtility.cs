using System;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


namespace ColloSys.UserInterface.Areas.Reporting.ViewModels
{
    public class ExportUtility
    {
        public static string ColumnLetter(int intCol)
        {
            int intFirstLetter = ((intCol) / 676) + 64;
            int intSecondLetter = ((intCol % 676) / 26) + 64;
            int intThirdLetter = (intCol % 26) + 65;

            char FirstLetter = (intFirstLetter > 64) ? (char)intFirstLetter : ' ';
            char SecondLetter = (intSecondLetter > 64) ? (char)intSecondLetter : ' ';
            char ThirdLetter = (char)intThirdLetter;

            return string.Concat(FirstLetter, SecondLetter, ThirdLetter).Trim();
        }

        // Create a text cell
        private static Cell CreateTextCell(string header, UInt32 index, string text)
        {
            var cell = new Cell { DataType = CellValues.InlineString, CellReference = header + index };
            var istring = new InlineString();
            var t = new Text { Text = text };
            istring.Append(t);
            cell.Append(istring);
            return cell;
        }

        public static MemoryStream GetExcel(IList<dynamic> DataList,string [] columns,string className)
        {
            MemoryStream stream = new MemoryStream();
            UInt32 rowcount = 0;
            PropertyInfo[] propertyInfo = new PropertyInfo[columns.Length];
            for(int _count = 0; _count < columns.Length; _count++) propertyInfo[_count] = commonUtility.GetType(className).GetProperty(columns[_count]);
            
            // Create the Excel document
            var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
            var workbookPart = document.AddWorkbookPart();
            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var relId = workbookPart.GetIdOfPart(worksheetPart);

            var workbook = new Workbook();
            var fileVersion = new FileVersion { ApplicationName = "Microsoft Office Excel" };
            var worksheet = new Worksheet();
            var sheetData = new SheetData();
            worksheet.Append(sheetData);
            worksheetPart.Worksheet = worksheet;

            var sheets = new Sheets();
            var sheet = new Sheet { Name = "Sheet1", SheetId = 1, Id = relId };
            sheets.Append(sheet);
            workbook.Append(fileVersion);
            workbook.Append(sheets);
            document.WorkbookPart.Workbook = workbook;
            document.WorkbookPart.Workbook.Save();

            // Add header to the sheet
            var row = new Row { RowIndex = ++rowcount };
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                row.Append(CreateTextCell(ColumnLetter(i), rowcount, propertyInfo[i].Name));
            }
            sheetData.AppendChild(row);
            worksheetPart.Worksheet.Save();

            // Add data to the sheet
            for (int i = 0; i < DataList.Count; i++)
            {
                row = new Row { RowIndex = ++rowcount };
            for (int j = 0; j < propertyInfo.Length; j++)
               {
            //        //dataRow[fieldsToExpose[i]].ToString()
                   //var y = typeof(T).InvokeMember(propertyInfo[j].Name, BindingFlags.GetProperty, null, DataList[i], null);
                   var y = commonUtility.GetType(className).InvokeMember(propertyInfo[j].Name, BindingFlags.GetProperty, null, DataList[i], null);
                   if (propertyInfo[j].PropertyType.Name.ToUpper() == "DATETIME" || propertyInfo[j].ToString().ToUpper().Contains("SYSTEM.DATETIME"))
                   {
                       row.Append(CreateTextCell(ColumnLetter(j), rowcount, (y == null) ? "" : Convert.ToDateTime(y.ToString()).ToString("yyyy-MM-ddTHH:mm:ssZ")));
                   }
                   else row.Append(CreateTextCell(ColumnLetter(j), rowcount, (y == null) ? "" : y.ToString()));
             }
                sheetData.AppendChild(row);
            }

            worksheetPart.Worksheet.Save();

            document.Close();
            return stream;
        }

        public static void GetCSV(StringBuilder str, IList<dynamic> DataList, string[] columns,string className)
        {
            //PropertyInfo[] propertyInfo = typeof(T).GetProperties();
            // = new StringBuilder();

            PropertyInfo[] propertyInfo = new PropertyInfo[columns.Length];
            for (int _count = 0; _count < columns.Length; _count++) propertyInfo[_count] = commonUtility.GetType(className).GetProperty(columns[_count]);

            for (int i = 0; i < propertyInfo.Length; i++)
            {
                str.Append(propertyInfo[i].Name + ',');
            }
            
            str.Append("\n");

            for (int j = 0; j < DataList.Count; j++)
            {
                foreach (PropertyInfo pi in propertyInfo)
                {
                    //this is the row+col intersection (the value)
                    string whatToWrite = string.Empty;
                    if (pi.PropertyType.Name.ToUpper() == "DATETIME" || pi.ToString().ToUpper().Contains("SYSTEM.DATETIME"))
                    {
                        whatToWrite = Convert.ToDateTime(DataList[j].GetType()
                                                    .GetProperty(pi.Name)
                                                    .GetValue(DataList[j], null)).ToString("yyyy-MM-ddTHH:mm:ssZ")
                                   .Replace(',', ' ') + ',';
                    }
                    else
                    {
                        //Try catch is added, if there is null value Convert.ToString function is giving error.
                        try
                        {
                            whatToWrite = Convert.ToString(DataList[j].GetType()
                                                     .GetProperty(pi.Name)
                                                     .GetValue(DataList[j], null))
                                    .Replace(',', ' ') + ',';
                        }
                        catch
                        {
                            whatToWrite = ",";
                        }
                    }
                    str.Append(whatToWrite);

                }
                str.Append("\n");
            }
        }
    }
}