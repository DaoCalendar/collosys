using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff
{
   public class RlsRWriteOffPlAebRecordCreator:AliasRWriteOffRecordCreator
    {
        private const uint AccountPosition = 1;
        private const uint AccountLength = 11;
        private const uint CycleString = 4;
       public RlsRWriteOffPlAebRecordCreator(FileScheduler fileScheduler)
           : base(fileScheduler,AccountPosition,AccountLength,CycleString)
       {
       }

     
    }
}
