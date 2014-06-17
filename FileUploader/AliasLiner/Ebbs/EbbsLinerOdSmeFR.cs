#region references

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Ebbs
{
// ReSharper disable once InconsistentNaming
   public class EbbsLinerOdSmeFR:EbbsLinerSharedFR
    {
       public EbbsLinerOdSmeFR(FileScheduler fileScheduler, IRecord<ELiner> recordCreator) : base(fileScheduler, recordCreator)
       {
       }

       public override bool PostProcessing()
       {
           return false;
       }
    }
}
