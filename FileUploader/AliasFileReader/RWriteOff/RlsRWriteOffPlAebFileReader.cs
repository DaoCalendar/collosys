using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasFileReader.RWriteOff
{
   public  class RlsRWriteOffPlAebFileReader:FileReader<RWriteoff>
    {
       public RlsRWriteOffPlAebFileReader(FileScheduler fileScheduler)
           :base(new RlsRWriteOffPlAebRecordCreator(fileScheduler))
       {
       }
    }
}
