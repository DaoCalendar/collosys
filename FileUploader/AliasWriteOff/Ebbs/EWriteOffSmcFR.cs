#region references

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Ebbs
{
// ReSharper disable once InconsistentNaming
    public class EWriteOffSmcFR : EWriteOffSharedFR
    {
        public EWriteOffSmcFR(FileScheduler fileScheduler, IRecord<EWriteoff> recordCreator) : base(fileScheduler, recordCreator)
        {
        }
    }
}
