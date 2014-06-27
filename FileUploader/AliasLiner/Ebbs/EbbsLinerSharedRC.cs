#region references

using System;
using System.Globalization;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.ExcelReader;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Ebbs
{
    // ReSharper disable once InconsistentNaming
    public abstract class EbbsLinerSharedRC : RecordCreator<ELiner>
    {
        private uint AccountNoPosition { get; set; }
        private uint AccountNoLegnth { get; set; }

        protected EbbsLinerSharedRC(uint accNo, uint accLength)
        {
            AccountNoPosition = accNo;
            AccountNoLegnth = accLength;
        }

        public override bool IsRecordValid(ELiner entity)
        {
            return true;
        }

        public override ELiner GetRecordForUpdate()
        {
            return new ELiner();
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
            if (preEntity == null)
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
                                           ? preEntity.DelqHistoryString.Substring(1) + entity.Bucket
                                           : preEntity.DelqHistoryString;

            if (YesterdayRecords.First().FileDate.Month != FileScheduler.FileDate.Month)
            {
                entity.Flag = ColloSysEnums.DelqFlag.N;
                entity.AccountStatus = ColloSysEnums.DelqAccountStatus.Norm;

                return true;
            }

            entity.Flag = ColloSysEnums.DelqFlag.O;
            entity.AccountStatus = ColloSysEnums.DelqAccountStatus.PEND;

            entity.BucketDue = preEntity.BucketDue;
            entity.Bucket1Due = preEntity.Bucket1Due;
            entity.Bucket2Due = preEntity.Bucket2Due;
            entity.Bucket3Due = preEntity.Bucket3Due;
            entity.Bucket4Due = preEntity.Bucket4Due;
            entity.Bucket5Due = preEntity.Bucket5Due;
            //entity.MinimumDue = priviousLiner.MinimumDue;
            return true;
        }

        public override ELiner GetPreviousDayEntity(ELiner entity)
        {
            return YesterdayRecords.SingleOrDefault(x => x.AccountNo == entity.AccountNo);
        }


    }
}
