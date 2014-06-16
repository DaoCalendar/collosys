#region ref

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Rls
{
// ReSharper disable once InconsistentNaming
   public class RLinerBfsFR:RLinerSharedFR
    {
       public RLinerBfsFR(FileScheduler fileScheduler, IRecord<RLiner> recordCreator) : base(fileScheduler, recordCreator)
       {
       }
    }
}
