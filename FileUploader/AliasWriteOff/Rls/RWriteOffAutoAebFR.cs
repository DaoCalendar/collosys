#region ref

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Rls
{
// ReSharper disable once InconsistentNaming
    public class RWriteOffAutoAebFR:RWriteOffSharedFR
    {
        public RWriteOffAutoAebFR(FileScheduler fileScheduler, IRecord<RWriteoff> recordCreator)
            : base(fileScheduler, recordCreator)
        {
        }
    }
}
