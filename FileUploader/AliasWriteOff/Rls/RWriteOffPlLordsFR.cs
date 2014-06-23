#region ref

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Rls
{
// ReSharper disable once InconsistentNaming
    public class RWriteOffPlLordsFR:RWriteOffSharedFR
    {
        public RWriteOffPlLordsFR(FileScheduler fileScheduler, IExcelRecord<RWriteoff> recordCreator)
            : base(fileScheduler, recordCreator)
        {
        }
    }
}
