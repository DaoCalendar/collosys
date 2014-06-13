using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasFileReader.RWriteOff
{
   public  class EbbsWriteOffSmcFileReader:FileReader<EWriteoff>
    {
       public EbbsWriteOffSmcFileReader(FileScheduler fileScheduler)
           : base(new EbbsRwriteOffSmcRecordCreator(fileScheduler))
       {
       }
    }
}
