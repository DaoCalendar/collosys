#region references

using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.FileUploadService.Interfaces;
using NLog;

#endregion

namespace ColloSys.FileUploadService.ExcelReader
{
    public class EWriteOffReader : SingleTableExcelReader<EWriteoff>, IExcelFile<EWriteoff>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region constructor

        private static FileReaderProperties GetFileReaderProperties()
        {
            return new FileReaderProperties
            {
                //HasFileDateInsideFile = true
            };
        }

        public EWriteOffReader(FileScheduler file)
            : base(file, GetFileReaderProperties())
        {
            TodayRecordList.AddEntities(Reader.GetDataLayer.GetDataForDate<EWriteoff>(Reader.UploadedFile.FileDate.Date));
        }
        #endregion

        #region Populate Computed Value

        public override bool PopulateComputedValue(EWriteoff record, out string errorDescription)
        {
            record.FileDate = Reader.UploadedFile.FileDate;
            //record.AmountRepaid = record.TotalDue - record.CurrentDue;
            errorDescription = string.Empty;
            return true;
        }

        #endregion

        #region overrided methods

        public override EWriteoff GetByUniqueKey(EWriteoff record)
        {
            return TodayRecordList.GetEntity(record.AccountNo, record.FileDate.Date);
        }

        protected override void AddRecordInListByUniqueKey(EWriteoff record, List<EWriteoff> recordList)
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
            // loan number must exist
            ulong loanNumber;
            if (!ulong.TryParse(dr[1].ToString(), out loanNumber) || (loanNumber.ToString(CultureInfo.InvariantCulture).Length < 5))
            {
                _log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", dr[1]));
                return false;
            }

            //dr[1] = (Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.E_WRITEOFF_SME)
            //            ? loanNumber.ToString("D8")
            //            : loanNumber.ToString("D11");

            dr[1] = loanNumber.ToString("D11");

            // product must be present
            var product = dr[7].ToString();
            if (string.IsNullOrWhiteSpace(product))
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

