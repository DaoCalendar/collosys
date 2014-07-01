#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
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
                entity.DelqHistoryString += entity.Bucket;
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

        public override void PostProcessing()
        {
            InsertIntoInfo();
        }

        private void InsertIntoInfo()
        {
            var infos = DbLayer.GetTableData<CustomerInfo>();

            ISet<CustomerInfo> isetInfo = new HashSet<CustomerInfo>(infos);

            //var todayLiner = OldDbRecordList.Where(x => x.FileDate.Date == Reader.UploadedFile.FileDate.Date).ToList();
            var todayLiner = TodayRecordList.GetEntities(FileScheduler.FileDate.Date);

            var saveEntity = new List<Entity>();
            foreach (var liner in todayLiner)
            {
                var info = isetInfo.FirstOrDefault(x => x.AccountNo == liner.AccountNo);

                if (info != null)
                {
                    info.AccountNo = liner.AccountNo;
                    info.CustomerName = liner.CustomerName;
                    info.Pincode = liner.Pincode;
                    info.Product = liner.Product;
                    info.IsInRecovery = false;
                    info.AllocStatus = liner.AllocStatus;
                    info.Bucket = liner.Bucket;
                    info.CustStatus = liner.CustStatus;
                    info.Cycle = liner.Cycle;
                    info.IsReferred = liner.IsReferred;
                    info.NoAllocResons = liner.NoAllocResons;
                    info.TotalDue = liner.TotalDue;
                    info.Flag = liner.Flag;
                }
                else
                {
                    info = new CustomerInfo
                    {
                        AccountNo = liner.AccountNo,
                        CustomerName = liner.CustomerName,
                        Pincode = liner.Pincode,
                        Product = liner.Product,
                        IsInRecovery = false,
                        AllocStatus = liner.AllocStatus,
                        Bucket = liner.Bucket,
                        CustStatus = liner.CustStatus,
                        Cycle = liner.Cycle,
                        IsReferred = liner.IsReferred,
                        NoAllocResons = liner.NoAllocResons,
                        TotalDue = liner.TotalDue,
                        Flag = liner.Flag
                    };
                }

                saveEntity.Add(info);
                isetInfo.Add(info);
            }

            DbLayer.SaveOrUpdateData(saveEntity);
        }
    }
}
