using System;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff
{
   public class EbbsRwriteOffAutoRecordCreator:AliasEWriteOffRecordCreator
   {
        private const uint AccountPosition = 1;
        private const uint AccountLength = 11;

        public EbbsRwriteOffAutoRecordCreator(FileScheduler fileScheduler)
            : base(fileScheduler, AccountPosition, AccountLength)
        {
        }

        public override bool GetCheckBasicField(IExcelReader reader, ICounter counter)
        {
            throw new NotImplementedException();
        }
    }
}
