#region ref

using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Rls
{
    // ReSharper disable once InconsistentNaming
    public class RWriteOffSmeFR : RWriteOffSharedFR
    {
        public RWriteOffSmeFR(FileScheduler fileScheduler)
            : base(fileScheduler, new RWriteOffSmeRC())
        {
            
        }
    }
}
