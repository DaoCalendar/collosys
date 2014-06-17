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
        public override bool ComputedSetter(ELiner entity)
        {
            ulong loanNumber;
            ulong.TryParse(Reader.GetValue(2), out loanNumber);
            entity.AccountNo = loanNumber.ToString("D11");

            // check date of reaging and date past due
            var dateofReagingExcel = Reader.GetValue(19).ToString(CultureInfo.InvariantCulture);
            var dayPastDueExcel = Reader.GetValue(21).ToString(CultureInfo.InvariantCulture);
            uint dayPastDue;
            if (string.IsNullOrWhiteSpace(dateofReagingExcel) && !uint.TryParse(dayPastDueExcel, out dayPastDue))
            {
                _log.Debug(string.Format("Data is rejected, Because DateOfReagin : {0} and DayPastDue : {1} is Empty", dateofReagingExcel, dayPastDueExcel));
                return false;
            }

            // check limit provn pdt code
            var limitProvnPdtCode =Reader.GetValue(15).ToString(CultureInfo.InvariantCulture);
            var product = DecodeScbProduct.GetEBBSProduct(limitProvnPdtCode);
            if (product == ScbEnums.Products.UNKNOWN)
            {
                _log.Debug(string.Format("Data is rejected, Because LimitProvnPdtCode : {0}", limitProvnPdtCode));
                return false;
            }

            // cycle
            entity.Cycle = Convert.ToUInt32(entity.CycleDate.Day);

            // CurrentDue
            //entity.CurrentDue = entity.CurrentBalance;

            // bucket
            entity.Bucket = GetBucketForELiner(entity) + 1;

            return true;
        }
    }
}
