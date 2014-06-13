using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff
{
   public class RlsRWriteOffAutoGbRecordCreator:AliasRWriteOffRecordCreator
    {
        private const uint AccountPosition = 1;
        private const uint AccountLength = 11;
        private const uint CycleString = 5;
       public RlsRWriteOffAutoGbRecordCreator(FileScheduler fileScheduler)
           : base(fileScheduler,AccountPosition,AccountLength,CycleString)
       {
       }

       
    }
}
