using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColloSys.FileUploader.ExcelReader
{
    public static class SharedUtility
    {
         public static bool IsDigit(string str)
        {
            return str.All(char.IsDigit);
        }
    }
}
