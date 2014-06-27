#region ref

using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Rls
{
// ReSharper disable once InconsistentNaming
    public class RWriteOffPlScbFR:RWriteOffSharedFR
    {
        public RWriteOffPlScbFR(FileScheduler fileScheduler)
            : base(fileScheduler, new RWriteOffPlScbRC())
        {
            
        }
    }
}
