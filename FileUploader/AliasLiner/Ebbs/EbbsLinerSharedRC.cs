#region references

using System;
using System.Globalization;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Ebbs
{
// ReSharper disable once InconsistentNaming
    public abstract class EbbsLinerSharedRC:RecordCreator<ELiner>
    {
        public override bool IsRecordValid(ELiner entity)
        {
           return true;
        }

        public override bool CheckBasicField()
        {
            // check loan number
            ulong loanNumber;
            if (!ulong.TryParse(Reader.GetValue(2), out loanNumber) || (loanNumber.ToString(CultureInfo.InvariantCulture).Length < 5))
            {
                _log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", Reader.GetValue(2)));
                return false;
            }
            return true;
        }

        public override ELiner GetRecordForUpdate()
        {
            throw new NotImplementedException();
        }

        protected static uint GetBucketForELiner(ELiner eLiner)
        {
            if (eLiner.DayPastDue <= 29)
                return 0;

            if (eLiner.DayPastDue <= 59)
                return 1;

            if (eLiner.DayPastDue <= 89)
                return 2;

            if (eLiner.DayPastDue <= 119)
                return 3;

            if (eLiner.DayPastDue <= 149)
                return 4;

            return (uint)(eLiner.DayPastDue <= 179 ? 5 : 6);
        }

        public override bool ComputedSetter(ELiner entity, ELiner preEntity)
        {
            var priviousLiner = PreviousDayLiner.SingleOrDefault(x => x.AccountNo == entity.AccountNo);
            if (priviousLiner == null)
            {
                entity.TotalDue = entity.CurrentDue;
                entity.PeakBucket = entity.Bucket;
                entity.DelqHistoryString = entity.Bucket.ToString("000000000000");
                entity.Flag = ColloSysEnums.DelqFlag.N;
                entity.AccountStatus = ColloSysEnums.DelqAccountStatus.Norm;
                return true;
            }

            entity.TotalDue = entity.CurrentDue;
            entity.PeakBucket = entity.Bucket;
            entity.DelqHistoryString = (entity.Cycle == FileScheduler.FileDate.Day)
                                           ? priviousLiner.DelqHistoryString.Substring(1) + entity.Bucket
                                           : priviousLiner.DelqHistoryString;

            if (PreviousDayLiner.First().FileDate.Month != FileScheduler.FileDate.Month)
            {
                entity.Flag = ColloSysEnums.DelqFlag.N;
                entity.AccountStatus = ColloSysEnums.DelqAccountStatus.Norm;

                return true;
            }

            entity.Flag = ColloSysEnums.DelqFlag.O;
            entity.AccountStatus = ColloSysEnums.DelqAccountStatus.PEND;

            entity.BucketDue = priviousLiner.BucketDue;
            entity.Bucket1Due = priviousLiner.Bucket1Due;
            entity.Bucket2Due = priviousLiner.Bucket2Due;
            entity.Bucket3Due = priviousLiner.Bucket3Due;
            entity.Bucket4Due = priviousLiner.Bucket4Due;
            entity.Bucket5Due = priviousLiner.Bucket5Due;
            //entity.MinimumDue = priviousLiner.MinimumDue;
            return true;
        }

    }
}
