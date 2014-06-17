#region ref

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Rls
{
// ReSharper disable once InconsistentNaming
   public class RLinerMortFR:RLinerSharedFR
    {
       public RLinerMortFR(FileScheduler fileScheduler) 
           : base(fileScheduler, new RLinerMortRC())
       {
       }

       public override bool PostProcessing()
       {
           return true;
       }
    }
}
