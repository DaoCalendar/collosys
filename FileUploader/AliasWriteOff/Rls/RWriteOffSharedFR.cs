#region ref

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Rls
{
// ReSharper disable once InconsistentNaming
    public class RWriteOffSharedFR:FileReader<RWriteoff>
    {
        public RWriteOffSharedFR(FileScheduler fileScheduler, IRecord<RWriteoff> recordCreator) : base(fileScheduler, recordCreator)
        {
        }
    }
}
