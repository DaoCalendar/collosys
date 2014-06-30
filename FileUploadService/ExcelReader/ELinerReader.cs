#region references

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.FileUploadService.BaseReader;
using ColloSys.FileUploadService.Interfaces;
using NLog;

#endregion

namespace ColloSys.FileUploadService.ExcelReader
{
    public class ELinerReader : SingleTableExcelReader<ELiner>, IExcelFile<ELiner>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly ISet<ELiner> _priviousDayLiner;

        #region constructor

        private static FileReaderProperties GetFileReaderProperties()
        {
            return new FileReaderProperties
                {
                    //HasFileDateInsideFile = false
                };
        }

        public ELinerReader(FileScheduler file)
            : base(file, GetFileReaderProperties())
        {
            TodayRecordList.AddEntities(Reader.GetDataLayer.GetDataForDate<ELiner>(Reader.UploadedFile.FileDate.Date));
            _priviousDayLiner = new HashSet<ELiner>();
            _priviousDayLiner.UnionWith(Reader.GetDataLayer.GetDataForPreviousDay<ELiner>(Reader.UploadedFile.FileDetail.AliasName, file.FileDate.Date, file.FileDetail.FileCount));

            if (_priviousDayLiner.Count <= 0)
                return;

            if (_priviousDayLiner.First().FileDate.Month != Reader.UploadedFile.FileDate.Month)
            {
                _priviousDayLiner = new HashSet<ELiner>();;
            }
        }

        #endregion

        #region interface methods

        public override bool PopulateComputedValue(ELiner record, out string errorDescription)
        {
            try
            {
                // cycle
                record.Cycle = Convert.ToUInt32(record.CycleDate.Day);

                // CurrentDue
                //record.CurrentDue = record.CurrentBalance;

                // bucket
                record.Bucket = GetBucketForELiner(record).ToString(CultureInfo.InvariantCulture);
                if (record.Product == ScbEnums.Products.AUTO_OD)
                {
                    record.Bucket = record.Bucket + 1;
                }


                var priviousLiner = _priviousDayLiner.SingleOrDefault(x => x.AccountNo == record.AccountNo);
                if (priviousLiner == null)
                {
                    record.TotalDue = record.CurrentDue;
                    record.PeakBucket = record.Bucket;
                    record.DelqHistoryString += record.Bucket;
                    record.Flag = ColloSysEnums.DelqFlag.N;
                    record.AccountStatus = ColloSysEnums.DelqAccountStatus.Norm;
                    errorDescription = string.Empty;
                    return true;
                }

                record.TotalDue = record.CurrentDue;
                record.PeakBucket = record.Bucket;
                record.DelqHistoryString = (record.Cycle == Reader.UploadedFile.FileDate.Day)
                                               ? priviousLiner.DelqHistoryString.Substring(1) + record.Bucket
                                               : priviousLiner.DelqHistoryString;
                
                if (_priviousDayLiner.First().FileDate.Month != Reader.UploadedFile.FileDate.Month)
                {
                    record.Flag = ColloSysEnums.DelqFlag.N;
                    record.AccountStatus = ColloSysEnums.DelqAccountStatus.Norm;

                    errorDescription = string.Empty;
                    return true;
                }

                record.Flag = ColloSysEnums.DelqFlag.O;
                record.AccountStatus = ColloSysEnums.DelqAccountStatus.PEND;

                record.BucketDue = priviousLiner.BucketDue;
                record.Bucket1Due = priviousLiner.Bucket1Due;
                record.Bucket2Due = priviousLiner.Bucket2Due;
                record.Bucket3Due = priviousLiner.Bucket3Due;
                record.Bucket4Due = priviousLiner.Bucket4Due;
                record.Bucket5Due = priviousLiner.Bucket5Due;
                //record.MinimumDue = priviousLiner.MinimumDue;

                //errorOnMapping = null;
                errorDescription = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                errorDescription = string.Format("Error On Computed Method : {0}", ex.Message);
                return true;
            }
        }

        public override ELiner GetByUniqueKey(ELiner record)
        {
            return TodayRecordList.GetEntity(record.AccountNo, record.FileDate.Date);
        }

        protected override void AddRecordInListByUniqueKey(ELiner record, List<ELiner> recordList)
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
            // check loan number
            ulong loanNumber;
            if (!ulong.TryParse(dr[2].ToString(), out loanNumber) || (loanNumber.ToString(CultureInfo.InvariantCulture).Length < 5))
            {
                _log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", dr[2]));
                return false;
            }
            dr[2] = loanNumber.ToString("D11");

            // if OD-SME no further checks
            if (Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.E_LINER_OD_SME)
            {
                return true;
            }

            // check date of reaging and date past due
            var dateofReagingExcel = dr[19].ToString();
            var dayPastDueExcel = dr[21].ToString();
            uint dayPastDue;
            if (string.IsNullOrWhiteSpace(dateofReagingExcel) && !uint.TryParse(dayPastDueExcel, out dayPastDue))
            {
                _log.Debug(string.Format("Data is rejected, Because DateOfReagin : {0} and DayPastDue : {1} is Empty", dateofReagingExcel, dayPastDueExcel));
                return false;
            }

            // check limit provn pdt code
            var limitProvnPdtCode = dr[15].ToString();
            var product = DecodeScbProduct.GetEBBSProduct(limitProvnPdtCode);
            if (product == ScbEnums.Products.UNKNOWN)
            {
                _log.Debug(string.Format("Data is rejected, Because LimitProvnPdtCode : {0}", limitProvnPdtCode));
                return false;
            }

            return true;
        }

        public override bool IsRecordValid(ELiner record, out string errorDescription)
        {

            errorDescription = string.Empty;
            return true;
        }

        public override bool PerformUpdates(ELiner record)
        {
            return true;
        }

        public override bool PostProcesing()
        {
            try
            {
                // insert Normalized Customer
                _log.Info("InsertNormalizedCustomer() start");
                InsertNormalizedCustomer();

                // insert Into info
                _log.Info("InsertIntoInfo() start");
                InsertIntoInfo();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void InsertNormalizedCustomer()
        {
            if (Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.E_LINER_OD_SME)
                return;

            if (_priviousDayLiner == null || _priviousDayLiner.Count <= 0)
            {
                return;
            }

            // for month start liner
            if (_priviousDayLiner.First().FileDate.Month != Reader.UploadedFile.FileDate.Month)
            {
                return;
            }

            var missingEnties = _priviousDayLiner.Where(x => !TodayRecordList.DoesAccountExist(x)).ToList();

            foreach (var liner in missingEnties)
            {
                liner.ResetUniqueProperties();
                liner.Flag = ColloSysEnums.DelqFlag.R;
                liner.AccountStatus = ColloSysEnums.DelqAccountStatus.Norm;
                liner.Allocs = null;
                liner.FileDate = Reader.UploadedFile.FileDate;
                liner.FileScheduler = Reader.UploadedFile;
                liner.FileRowNo = 0;
                liner.MinimumDue = 0;
                Reader.Counter.AddUploadRecord(0);
            }

            Reader.GetDataLayer.SaveOrUpdateData(missingEnties);
        }

        private void InsertIntoInfo()
        {
            var infos = Reader.GetDataLayer.GetTableData<CustomerInfo>();

            ISet<CustomerInfo> isetInfo = new HashSet<CustomerInfo>(infos);

            //var todayLiner = OldDbRecordList.Where(x => x.FileDate.Date == Reader.UploadedFile.FileDate.Date).ToList();
            var todayLiner = TodayRecordList.GetEntities(Reader.UploadedFile.FileDate.Date);

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

            Reader.GetDataLayer.SaveOrUpdateData(saveEntity);
        }

        #endregion

        #region private methods

        private static uint GetBucketForELiner(ELiner eLiner)
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

        #endregion
    }
}
