using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff
{
   public class RlsRWriteOffAutoGbRecordCreator:AliasRWriteOffRecordCreator
    {
       public RlsRWriteOffAutoGbRecordCreator(FileScheduler fileScheduler)
           : base(fileScheduler)
       {
       }

       
    }
}
