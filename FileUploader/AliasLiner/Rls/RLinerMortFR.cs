#region ref

using System;
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
           try
           {
               RecordCreatorObj.PostProcessing();
           }
           catch (Exception exception)
           {
               Log.Error("In PostProcessing of RLiner MORT:" + exception.Message);
               return false;
           }

           return true;
       }
    }
}
