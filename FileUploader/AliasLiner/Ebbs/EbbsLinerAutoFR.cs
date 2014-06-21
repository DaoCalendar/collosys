#region references

using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.RecordManager;

#endregion


namespace ColloSys.FileUploaderService.AliasLiner.Ebbs
{
// ReSharper disable once InconsistentNaming
    public class EbbsLinerAutoFR:EbbsLinerSharedFR
    {
        public EbbsLinerAutoFR(FileScheduler fileScheduler) 
            : base(fileScheduler, new EbbsLinerAutoRC())
        {
            
        }

        public override bool PostProcessing()
        {
            InsertNormalizedCustomer();
            return true;
        }

        private void InsertNormalizedCustomer()
        {
            if (_objRecord.PreviousDayLiner == null || _objRecord.PreviousDayLiner.Count <= 0)
            {
                return;
            }

            // for month start liner
            if (_objRecord.PreviousDayLiner.First().FileDate.Month != FileScheduler.FileDate.Month)
            {
                return;
            }

            var missingEnties = _objRecord.PreviousDayLiner.Where(x => !TodayRecordList.DoesAccountExist(x)).ToList();

            foreach (var liner in missingEnties)
            {
                liner.ResetUniqueProperties();
                liner.Flag = ColloSysEnums.DelqFlag.R;
                liner.AccountStatus = ColloSysEnums.DelqAccountStatus.Norm;
                liner.FileDate = FileScheduler.FileDate;
                liner.FileScheduler = FileScheduler;
                liner.FileRowNo = 0;
                liner.MinimumDue = 0;
                _counter.IncrementInsertRecords();
            }

            _DbLayer.SaveOrUpdateData(missingEnties);
        }
    }
}
