#region references

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Ebbs
{
// ReSharper disable once InconsistentNaming
   public class EbbsLinerOdSmeFR:EbbsLinerSharedFR
    {
       public EbbsLinerOdSmeFR(FileScheduler fileScheduler) : base(fileScheduler, new EbbsLinerOdSmeRC())
       {
       }

       public override bool PostProcessing()
       {
           return false;
       }
    }
}
