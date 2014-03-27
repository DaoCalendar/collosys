using System.Globalization;
using NPOI.SS.UserModel;

namespace ReflectionExtension.ExcelReader
{
    public static class NPOIExtensions
    {
        public static string GetValue(this ICell cell, CellType cellType)
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

    }
}