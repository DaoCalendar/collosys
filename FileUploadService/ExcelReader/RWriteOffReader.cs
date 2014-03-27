#region references

using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.FileUploadService.Interfaces;
using NLog;

#endregion

namespace ColloSys.FileUploadService.ExcelReader
{
    public class RWriteOffReader : SingleTableExcelReader<RWriteoff>, IExcelFile<RWriteoff>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region constructor

        private static FileReaderProperties GetFileReaderProperties()
        {
            return new FileReaderProperties
            {
                //HasFileDateInsideFile = false
            };
        }

        public RWriteOffReader(FileScheduler file)
            : base(file, GetFileReaderProperties())
        {
            TodayRecordList.AddEntities(Reader.GetDataLayer.GetDataForDate<RWriteoff>(Reader.UploadedFile.FileDate.Date));
        }

        #endregion

        #region Populate Computed Value

        public override bool PopulateComputedValue(RWriteoff record, out string errorDescription)
        {
            record.FileDate = Reader.UploadedFile.FileDate;
            errorDescription = string.Empty;
            return true;
        }

        public override bool IsRecordValid(RWriteoff record, out string errorDescription)
        {
            errorDescription = string.Empty;
            return true;
        }

        public override bool PerformUpdates(RWriteoff record)
        {
            return true;
        }

        public override RWriteoff GetByUniqueKey(RWriteoff record)
        {
            return TodayRecordList.GetEntity(record.AccountNo, record.FileDate.Date);
        }

        protected override void AddRecordInListByUniqueKey(RWriteoff record, List<RWriteoff> recordList)
        {
            var index = recordList.FindIndex(x => x.FileDate == record.FileDate
                                             && x.AccountNo == record.AccountNo);

            if (index > -1)
            {
                recordList[index] = record;
                Reader.Counter.AddDuplicateRecord(RowNo);
                return;
            }

            recordList.Add(record);
        }

        public override bool CheckBasicField(DataRow dr)
        {
            ulong loanNumber;
            if (!ulong.TryParse(dr[1].ToString(), out loanNumber) || (loanNumber.ToString(CultureInfo.InvariantCulture).Length < 3))
            {
                _log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", dr[1]));
                return false;
            }

            dr[1] = loanNumber.ToString("D8");

            var cycleString = dr[5].ToString();

            if (Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_AEB ||
                Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_SCB ||
                Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.R_WRITEOFF_SME)
            {
                cycleString = dr[4].ToString();
            }

            uint cycle;
            if (!uint.TryParse(cycleString, out cycle))
            {
                return false;
            }

            return true;
        }

        public override bool PostProcesing()
        {
            // insert Into info
            _log.Info("InsertIntoInfo() start");
            InsertIntoInfo();

            return true;
        }

        private void InsertIntoInfo()
        {
            var infos = Reader.GetDataLayer.GetTableData<CustomerInfo>();

            ISet<CustomerInfo> isetInfo = new HashSet<CustomerInfo>(infos);

            //var todayWriteoff = OldDbRecordList.Where(x => x.FileDate.Date == Reader.UploadedFile.FileDate.Date).ToList();
            var todayWriteoff = TodayRecordList.GetEntities(Reader.UploadedFile.FileDate.Date);

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

            Reader.GetDataLayer.SaveOrUpdateData(saveEntity);
        }

        #endregion
    }
}
