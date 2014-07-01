#region ref

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.FileUploaderService.RecordManager;
using NLog;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Rls
{
    // ReSharper disable once InconsistentNaming
    public abstract class RWriteOffSharedRC : RecordCreator<RWriteoff>
    {
        private uint AccountPos { get; set; }
        private uint AccountNoLength { get; set; }
        private uint CycleString { get; set; }
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public RWriteOffSharedRC(uint accPos, uint accNolength, uint cyclestring)
        {
            AccountPos = accPos;
            AccountNoLength = accNolength;
            CycleString = cyclestring;
        }

        public override bool ComputedSetter(RWriteoff entity)
        {
            entity.FileDate = FileScheduler.FileDate;
            entity.Allocs = null;
            //if (entity.Settlement=="Y")
            //{
            //    entity.IsSetteled = true;
            //}
            entity.IsReferred = false;

            return true;
        }

        public override bool ComputedSetter(RWriteoff entity, RWriteoff preEntity)
        {
            return true;
        }

        public override bool IsRecordValid(RWriteoff entity)
        {
            return true;
        }

        public override bool CheckBasicField()
        {
            // loan no should be a number
            ulong loanNumber;
            if (!ulong.TryParse(Reader.GetValue(AccountPos), out loanNumber))
            {
                // Counter.IncrementIgnoreRecord();
                _log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", Reader.GetValue(AccountPos)));
                return false;
            }

            var cycleString = Reader.GetValue(CycleString);
            uint cycle;
            if (!uint.TryParse(cycleString, out cycle))
            {
                return false;
            }

            // loan number must be of 2 digits min
            return (loanNumber.ToString(CultureInfo.InvariantCulture).Length >= 2);
        }

        public override RWriteoff GetRecordForUpdate()
        {
            var accno = Reader.GetValue(AccountPos).ToString(CultureInfo.InvariantCulture);
            return TodayRecordList.GetEntity(accno) ??
                new RWriteoff();
            //return DbLayer.GetRecordForUpdate<RWriteoff>(Reader.GetValue(AccountPos).ToString(CultureInfo.InvariantCulture)) 
            //    ?? new RWriteoff();
        }

        public override RWriteoff GetPreviousDayEntity(RWriteoff entity)
        {
            throw new NotImplementedException();
        }

        public override void PostProcessing()
        {
            InsertIntoInfo();
        }

        private void InsertIntoInfo()
        {
            var infos = DbLayer.GetTableData<CustomerInfo>();

            ISet<CustomerInfo> isetInfo = new HashSet<CustomerInfo>(infos);

            //var todayWriteoff = OldDbRecordList.Where(x => x.FileDate.Date == Reader.UploadedFile.FileDate.Date).ToList();
            var todayWriteoff = TodayRecordList.GetEntities(FileScheduler.FileDate.Date);

            var saveEntity = new List<Entity>();
            foreach (var writeoff in todayWriteoff)
            {
                var info = isetInfo.FirstOrDefault(x => x.AccountNo == writeoff.AccountNo);

                if (info != null)
                {
                    info.AccountNo = writeoff.AccountNo;
                    info.CustomerName = writeoff.CustomerName;
                    info.Pincode = writeoff.Pincode;
                    info.Product = writeoff.Product;
                    info.IsInRecovery = true;
                    info.ChargeofDate = writeoff.ChargeOffDate;
                    info.AllocStatus = writeoff.AllocStatus;
                    info.CustStatus = writeoff.CustStatus;
                    info.Cycle = writeoff.Cycle;
                    info.IsReferred = writeoff.IsReferred;
                    info.NoAllocResons = writeoff.NoAllocResons;
                    info.TotalDue = writeoff.TotalDue;
                    info.Flag = ColloSysEnums.DelqFlag.N;
                }
                else
                {
                    info = new CustomerInfo
                    {
                        AccountNo = writeoff.AccountNo,
                        CustomerName = writeoff.CustomerName,
                        Pincode = writeoff.Pincode,
                        Product = writeoff.Product,
                        IsInRecovery = true,
                        ChargeofDate = writeoff.ChargeOffDate,
                        AllocStatus = writeoff.AllocStatus,
                        CustStatus = writeoff.CustStatus,
                        Cycle = writeoff.Cycle,
                        IsReferred = writeoff.IsReferred,
                        NoAllocResons = writeoff.NoAllocResons,
                        TotalDue = writeoff.TotalDue,
                        Flag = ColloSysEnums.DelqFlag.N
                    };
                }

                saveEntity.Add(info);
                isetInfo.Add(info);
            }

            DbLayer.SaveOrUpdateData(saveEntity);
        }
    }
}
