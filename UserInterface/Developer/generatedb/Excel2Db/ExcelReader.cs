#region reference

using Excel;
using System.Data;

#endregion

namespace ColloSys.UserInterface.Areas.Developer.Models.Excel2Db
{
    static class ExcelReader
    {
        public static DataSet ReadExcel(string fileName)
        {
            var stream = ResourceReader.GetEmbeddedResourceAsStream(fileName);
            using (var excelReader = ExcelReaderFactory.CreateBinaryReader(stream))
            {
                excelReader.IsFirstRowAsColumnNames = true;
                return excelReader.AsDataSet();
            }
        }
    }
}
