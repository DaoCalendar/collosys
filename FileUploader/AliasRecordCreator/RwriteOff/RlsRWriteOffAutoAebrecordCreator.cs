using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff
{
    public class RlsRRWriteOffAutoAebrecordCreator : AliasRWriteOffRecordCreator
    {
        private const uint AccountPosition = 1;
        private const uint AccountLength = 11;
        private const uint CycleString = 4;

        public RlsRRWriteOffAutoAebrecordCreator(FileScheduler file)
            : base(file,AccountPosition,AccountLength,CycleString)
        {
        }

       

    }
}
