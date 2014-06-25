using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;
using ColloSys.FileUploaderService.RecordManager;

namespace ColloSys.FileUploaderService.AliasTextReader
{
   public class CWriteOffFileReader:FileReader<CWriteoff>
   {
       public CWriteOffFileReader(FileScheduler fileScheduler) 
           : base(fileScheduler, new CWriteOffRecordCreator())
       {
       }

       public override bool PostProcessing()
       {
           throw new NotImplementedException();
       }
   }
}
