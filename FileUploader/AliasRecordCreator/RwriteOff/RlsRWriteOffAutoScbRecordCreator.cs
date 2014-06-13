using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff
{
    public  class RlsRWriteOffAutoScbRecordCreator:AliasRWriteOffRecordCreator
    {
        private const uint AccountPosition = 1;
        private const uint AccountLength = 11;
        private const uint CycleString = 4;
        public RlsRWriteOffAutoScbRecordCreator(FileScheduler fileScheduler)
            :base(fileScheduler,AccountPosition,AccountLength,CycleString)
        {

        }


        
    }
}
