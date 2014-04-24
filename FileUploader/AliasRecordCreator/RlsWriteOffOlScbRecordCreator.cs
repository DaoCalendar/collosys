using ColloSys.DataLayer.Domain;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.AliasReader
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
