#region ref

using System;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Rls
{
// ReSharper disable once InconsistentNaming
   public class RLinerPlFR:RLinerSharedFR
    {
       public RLinerPlFR(FileScheduler fileScheduler) 
           : base(fileScheduler, new RLinerPlRC())
       {
       }

       public override bool PostProcessing()
       {
           try
           {
               InsertResolvedCustomer();
               RecordCreatorObj.PostProcessing();
           }
           catch (Exception exception)
           {
               Log.Error("In PostProcessing of RLiner BFS:" + exception.Message);
               return false;
           }

           return true;
       }

       private void InsertResolvedCustomer()
       {
           if (RecordCreatorObj.YesterdayRecords == null || RecordCreatorObj.YesterdayRecords.Count <= 0)
           {
               return;
           }

           //for month start liner
           if (RecordCreatorObj.YesterdayRecords.First().FileDate.Month != FileScheduler.FileDate.Month)
           {
               return;
           }

           var missingEnties = RecordCreatorObj.YesterdayRecords.Where(x => !RecordCreatorObj.TodayRecordList.DoesAccountExist(x)).ToList();

           foreach (var liner in missingEnties)
           {
               liner.ResetUniqueProperties();
               liner.Flag = ColloSysEnums.DelqFlag.R;
               liner.FileDate = FileScheduler.FileDate;
               liner.FileScheduler = FileScheduler;
               liner.FileRowNo = 0;
               liner.Allocs = null;
               Counter.IncrementInsertRecords();
           }

           DbLayer.SaveOrUpdateData(missingEnties);
       }
    }
}
