using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasRecordCreator;
using ColloSys.FileUploader.RowCounter;
using ColloSys.FileUploaderService.AliasRecordCreator;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.AliasReader
{
    public class RlsWriteOffAutoAebrecordCreator : AliasWriteOffRecordCreator
    {

        public RlsWriteOffAutoAebrecordCreator(FileScheduler file)
            : base(file, 1, 8)
        {
        }

        public override bool GetCheckBasicField(IExcelReader reader, ICounter counter)
        {
            var cycleString = reader.GetValue(4);

            uint cycle;
            return uint.TryParse(cycleString, out cycle);
        }

    }
}
