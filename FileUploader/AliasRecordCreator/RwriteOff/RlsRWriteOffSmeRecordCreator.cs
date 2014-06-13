using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff
{
    public class RlsRWriteOffSmeRecordCreator:AliasRWriteOffRecordCreator
    {
        public RlsRWriteOffSmeRecordCreator(FileScheduler fileScheduler)
            : base(fileScheduler)
        {
        }

       
    }
}
