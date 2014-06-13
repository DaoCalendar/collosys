using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploaderService.AliasRecordCreator
{
    public class RlsWriteOffPlScbRecordCreator : AliasWriteOffRecordCreator
    {

        public RlsWriteOffPlScbRecordCreator(FileScheduler file)
            : base(file, 1, 8)
        {

        }

        public override bool GetCheckBasicField(IExcelReader reader, ICounter counter)
        {
            var cycleString = reader.GetValue(5);

            uint cycle;
            return uint.TryParse(cycleString, out cycle);
        }
    }
}
