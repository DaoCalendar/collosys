#region ref

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
           InsertResolvedCustomer();
           return true;
       }

       private void InsertResolvedCustomer()
       {
           if (_objRecord.PreviousDayLiner == null || _objRecord.PreviousDayLiner.Count <= 0)
           {
               return;
           }

           //for month start liner
           if (_objRecord.PreviousDayLiner.First().FileDate.Month != FileScheduler.FileDate.Month)
           {
               return;
           }

           var missingEnties = _objRecord.PreviousDayLiner.Where(x => !TodayRecordList.DoesAccountExist(x)).ToList();

           foreach (var liner in missingEnties)
           {
               liner.ResetUniqueProperties();
               liner.Flag = ColloSysEnums.DelqFlag.R;
               liner.FileDate = FileScheduler.FileDate;
               liner.FileScheduler = FileScheduler;
               liner.FileRowNo = 0;
               liner.Allocs = null;
               _counter.IncrementInsertRecords();
           }

           _DbLayer.SaveOrUpdateData(missingEnties);
       }
    }
}
