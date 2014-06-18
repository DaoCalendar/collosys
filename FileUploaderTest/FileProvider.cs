using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtension.Tests
{
   public  class FileProvider
    {
       public FileInfo FileInfoForDrillDown { get; set; }
       public FileInfo FileInfoForWoAuto { get; set; }
       public FileInfo FileInfoForLinerOdSme { get; set; }

       public FileProvider()
       {
           //Adding File Path for testing
           string filePath = "./ExcelData/DrillDown_Txn_1.xls";
           string filePathForWoAuto = "./ExcelData/AEB Auto Charge Off Base - 28.01.2014.xls";
           string filePathForLinerOdSme = "./ExcelData/AEB Auto Charge Off Base - 28.01.2014.xls";

           //FileInfo for testing
           FileInfoForDrillDown=new FileInfo(filePath);
           FileInfoForWoAuto = new FileInfo(filePathForWoAuto);
           FileInfoForLinerOdSme = new FileInfo(filePathForLinerOdSme);

       }


    }
}
