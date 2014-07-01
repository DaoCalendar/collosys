#region references

using System;
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
           try
           {
               RecordCreatorObj.PostProcessing();
           }
           catch (Exception exception)
           {
               Log.Error(string.Format("Error in PostProcessing of Ebbs Liner Od Sme FR: {0}",exception));
               return false;
           }
           return true;
       }
    }
}
