using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff
{
   public class RlsRWriteOffAutoRecordCreator:AliasRWriteOffRecordCreator
    {
       public RlsRWriteOffAutoRecordCreator(FileScheduler fileScheduler)
           : base(fileScheduler)
       {
       }

       
    }
}
