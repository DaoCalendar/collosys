#region references

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Ebbs
{
// ReSharper disable once InconsistentNaming
    public class EWriteOffSharedFR:FileReader<EWriteoff>
    {
        public EWriteOffSharedFR(FileScheduler fileScheduler, IExcelRecord<EWriteoff> recordCreator) 
            : base(fileScheduler, recordCreator)
        {
        }

        public override bool PostProcessing()
        {
            RecordCreatorObj.PostProcessing();
            return true;
        }
    }
}
