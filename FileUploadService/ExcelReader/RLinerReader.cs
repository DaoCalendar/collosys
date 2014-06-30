#region references

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.FileUploadService.Interfaces;
using ColloSys.FileUploadService.BaseReader;
using NLog;

#endregion

namespace ColloSys.FileUploadService.ExcelReader
{
    public class RLinerReader : SingleTableExcelReader<RLiner>, IExcelFile<RLiner>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly ISet<RLiner> _priviousDayLiner;

        #region constructor

        private static FileReaderProperties GetFileReaderProperties(FileScheduler file)
        {
            var properties = new FileReaderProperties
            {
                //HasFileDateInsideFile = true
            };

            //if (file.FileDetail.AliasName == ColloSysEnums.FileAliasName.E_LINER_OD_SME)
            //{
            //    properties.HasFileDateInsideFile = false;
            //}

            return properties;

        }

        public RLinerReader(FileScheduler file)
            : base(file, GetFileReaderProperties(file))
        {
            TodayRecordList.AddEntities(Reader.GetDataLayer.GetDataForDate<RLiner>(Reader.UploadedFile.FileDate.Date));

            if (Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.R_LINER_PL)
            {
                _priviousDayLiner = new HashSet<RLiner>();
                _priviousDayLiner.UnionWith(Reader.GetDataLayer
                                                  .GetDataForPreviousDay<RLiner>(
                                                      Reader.UploadedFile.FileDetail.AliasName,
                                                      file.FileDate.Date, file.FileDetail.FileCount));
            }
        }

        #endregion

        #region Populate Computed Value

        public override bool PopulateComputedValue(RLiner record, out string errorDescription)
        {
            record.FileDate = Reader.UploadedFile.FileDate;
            record.Bucket = GetRLinerBucketNumber(record.AgeCode).ToString(CultureInfo.InvariantCulture);

            if (Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.R_LINER_BFS_LOAN)
            {
                record.Product = DecodeScbProduct.GetRlsBFSProduct(record.ProductCode);
            }

            if (Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.R_LINER_MORT_LOAN)
            {
                record.Product = DecodeScbProduct.GetRlsMORTProduct(record.ProductCode);
            }

            if (Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.R_LINER_PL)
            {
                var priviousLiner = _priviousDayLiner.SingleOrDefault(x => x.AccountNo == record.AccountNo);
                if (priviousLiner == null)
                {
                    record.DelqHistoryString += record.Bucket;
                    record.Flag = ColloSysEnums.DelqFlag.N;
                    errorDescription = string.Empty;
                    return true;
                }

                record.DelqHistoryString = (record.Cycle == Reader.UploadedFile.FileDate.Day)
                                               ? priviousLiner.DelqHistoryString.Substring(1) + record.Bucket
                                               : priviousLiner.DelqHistoryString;
                


                //for month start liner
                if (_priviousDayLiner.First().FileDate.Month != Reader.UploadedFile.FileDate.Month)
                {
                    record.Flag = ColloSysEnums.DelqFlag.N;

                    errorDescription = string.Empty;
                    return true;
                }

                record.Flag = ColloSysEnums.DelqFlag.O;
            }

            errorDescription = string.Empty;
            return true;
        }

        #endregion

        #region overrided methods

        public override RLiner GetByUniqueKey(RLiner record)
        {
            return TodayRecordList.GetEntity(record.AccountNo, record.FileDate.Date);
        }

        protected override void AddRecordInListByUniqueKey(RLiner record, List<RLiner> recordList)
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

            return true;
        }

        public override bool PostProcesing()
        {
            if (Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.R_LINER_PL)
            {
                // insert Resolved Customer
                _log.Info("InsertResolvedCustomer() start");
                InsertResolvedCustomer();
            }

            // insert Into info
            _log.Info("InsertIntoInfo() start");
            InsertIntoInfo();

            return true;
        }

        private void InsertResolvedCustomer()
        {
            if (_priviousDayLiner == null || _priviousDayLiner.Count <= 0)
            {
                return;
            }

            //for month start liner
            if (_priviousDayLiner.First().FileDate.Month != Reader.UploadedFile.FileDate.Month)
            {
                return;
            }

            var missingEnties = _priviousDayLiner.Where(x => !TodayRecordList.DoesAccountExist(x)).ToList();

            foreach (var liner in missingEnties)
            {
                liner.ResetUniqueProperties();
                liner.Flag = ColloSysEnums.DelqFlag.R;
                liner.FileDate = Reader.UploadedFile.FileDate;
                liner.FileScheduler = Reader.UploadedFile;
                liner.FileRowNo = 0;
                liner.Allocs = null;
                Reader.Counter.AddUploadRecord(0);
            }

            Reader.GetDataLayer.SaveOrUpdateData(missingEnties);
            _log.Info("Total Resolved Customer : " + missingEnties.Count);
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
                        Flag=liner.Flag
                    };
                }

                saveEntity.Add(info);
                isetInfo.Add(info);
            }

            Reader.GetDataLayer.SaveOrUpdateData(saveEntity);
        }

        #endregion

        #region Helper
        private uint GetRLinerBucketNumber(string bucket)
        {
            bucket = bucket.Replace(",", "");

            if (Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.E_LINER_OD_SME)
            {
                if (string.IsNullOrWhiteSpace(bucket) || Convert.ToUInt32(bucket) == 0)
                    return 1;

                return (Convert.ToUInt32(bucket) / 30) + 1;
            }

            uint bucketNo;
            var data = new Dictionary<string, uint>();
            for (uint i = 65, j = 1; i <= 90; i++, j++)
            {
                data.Add(((char)i).ToString(CultureInfo.InvariantCulture), j);
            }
            if (Regex.IsMatch(bucket, @"^[a-zA-Z]+$"))
            {
                bucketNo = data[bucket.ToUpper()];
            }
            else
            {
                throw new FormatException("Given Bucket is not in well format");
            }
            return bucketNo-1;
        }
        #endregion
    }
}
