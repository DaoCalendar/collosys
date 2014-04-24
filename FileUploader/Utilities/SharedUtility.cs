using System;
using System.IO;
using System.Linq;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.Utilities
{
    public static class SharedUtility
    {
        private static IExcelReader _iExcelReader;
     

        public static bool IsDigit(string str)
        {
            return str.All(char.IsDigit);
        }
         public static IExcelReader GetInstance(FileStream fileInfo)
         {
             var file = Path.GetExtension(fileInfo.Name);
             switch (file)
             {
                 case ".xlsx":
                     return _iExcelReader = new EpPlusExcelReader(fileInfo);
                 case ".xls":
                     return _iExcelReader = new NpOiExcelReader(fileInfo);
                 default:
                     throw new Exception("Invalid File");
             }
         }
         public static IExcelReader GetInstance(FileInfo fileInfo)
         {
             var file = Path.GetExtension(fileInfo.Name);
             switch (file)
             {
                 case ".xlsx":
                     return _iExcelReader = new EpPlusExcelReader(fileInfo);
                 case ".xls":
                     return _iExcelReader = new NpOiExcelReader(fileInfo);
                 default:
                     throw new Exception("Invalid File");
             }
         }
    }
}
