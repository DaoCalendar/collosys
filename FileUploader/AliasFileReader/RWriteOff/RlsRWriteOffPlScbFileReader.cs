using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasFileReader.RWriteOff
{
   public  class RlsRWriteOffPlScbFileReader:FileReader<RWriteoff>
    {
       public RlsRWriteOffPlScbFileReader(FileScheduler fileScheduler)
           : base(new RlsRWriteOffPlScbRecordCreator(fileScheduler))
       {

       }
    }
}
