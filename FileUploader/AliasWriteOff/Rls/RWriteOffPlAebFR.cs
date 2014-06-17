#region ref

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Rls
{
// ReSharper disable once InconsistentNaming
    public class RWriteOffPlAebFR:RWriteOffSharedFR
    {
        public RWriteOffPlAebFR(FileScheduler fileScheduler)
            : base(fileScheduler, new RWriteOffPlAebRC())
        {
        }
    }
}
