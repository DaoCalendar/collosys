using System;
using System.IO;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasTextReader
{
   public class LitUitFileReader: FileReader<Payment>
   {
       public  static StreamReader _inpuStreamReader;
       public LitUitFileReader(FileScheduler fileScheduler)
           : base(fileScheduler, new LitUitRecordCreator(fileScheduler))
       {
          
       }

       public override bool PostProcessing()
       {
           throw new NotImplementedException();
       }
   }
}
