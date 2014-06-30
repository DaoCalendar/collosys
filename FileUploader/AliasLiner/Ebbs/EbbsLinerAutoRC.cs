#region references

using System;
using System.Globalization;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Ebbs
{
    // ReSharper disable once InconsistentNaming
    public class EbbsLinerAutoRC : EbbsLinerSharedRC
    {
        private const uint AccountNoPosition = 2;
        private const uint AccountNoLength = 11;
        public EbbsLinerAutoRC()
            :base(AccountNoPosition,AccountNoLength)
        {
            HasMultiDayComputation = true;
            var dbLayer = new DataLayer.DbLayer();
            //PreviousDayLiner = dbLayer.GetDataForPreviousDay<ELiner>(ColloSysEnums.FileAliasName.E_LINER_OD_SME,
            //    FileScheduler.FileDate, FileScheduler.FileDetail.FileCount);
        }
        public override bool ComputedSetter(ELiner entity)
        {
            ulong loanNumber;
            ulong.TryParse(Reader.GetValue(2), out loanNumber);
            entity.AccountNo = loanNumber.ToString("D11");

            var limitProvnPdtCode = Reader.GetValue(15).ToString(CultureInfo.InvariantCulture);
            entity.Product = DecodeScbProduct.GetEBBSProduct(limitProvnPdtCode);

            // cycle
            entity.Cycle = Convert.ToUInt32(entity.CycleDate.Day);

            // CurrentDue
            //entity.CurrentDue = entity.CurrentBalance;

            // bucket
            entity.Bucket = GetBucketForELiner(entity) + 1;

            

            return true;
        }

        public override bool CheckBasicField()
        {
            // check loan number
            ulong loanNumber;
            if (!ulong.TryParse(Reader.GetValue(AccountNoPosition), out loanNumber))
            {
                //Counter.IncrementIgnoreRecord();
                Log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", Reader.GetValue(AccountNoPosition)));
                return false;
            }

            // check date of reaging and date past due
            var dateofReagingExcel = Reader.GetValue(19).ToString(CultureInfo.InvariantCulture);
            var dayPastDueExcel = Reader.GetValue(21).ToString(CultureInfo.InvariantCulture);
            uint dayPastDue;
            if (string.IsNullOrWhiteSpace(dateofReagingExcel) && !uint.TryParse(dayPastDueExcel, out dayPastDue))
            {
                Log.Debug(string.Format("Data is rejected, Because DateOfReagin : {0} and DayPastDue : {1} is Empty", dateofReagingExcel, dayPastDueExcel));
                return false;
            }

            // check limit provn pdt code
            var limitProvnPdtCode = Reader.GetValue(15).ToString(CultureInfo.InvariantCulture);
            var product = DecodeScbProduct.GetEBBSProduct(limitProvnPdtCode);
            if (product == ScbEnums.Products.UNKNOWN)
            {
                Log.Debug(string.Format("Data is rejected, Because LimitProvnPdtCode : {0}", limitProvnPdtCode));
                return false;
            }
            return true;
        }
    }
}
