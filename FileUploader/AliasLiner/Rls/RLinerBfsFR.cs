#region ref

using System;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Rls
{
// ReSharper disable once InconsistentNaming
   public class RLinerBfsFR:RLinerSharedFR
    {
       public RLinerBfsFR(FileScheduler fileScheduler)
           : base(fileScheduler, new RLinerBfsRC())
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
               Log.Error("In PostProcessing of RLiner BFS:"+ exception.Message);
               return false;
           }
           
           return true;
       }
    }
}
