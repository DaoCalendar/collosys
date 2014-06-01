using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.AliasRecordCreator
{
    class DhflRecodCreator : AliasDHFLrecordCreator
    {
        private const uint AccountPosition = 2;
        private const uint AccountLength = 8;

        public DhflRecodCreator(FileScheduler fileScheduler)
            : base(fileScheduler, AccountPosition, AccountLength)
        {
        }

        public override bool GetComputations(DHFL_Liner record, IExcelReader reader)
        {
            return true;
        }
    }
}
