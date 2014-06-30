#region references

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.FileUploadService.Interfaces;
using ColloSys.Shared.SharedUtils;
using NLog;
using ColloSys.DataLayer.Services.Extensions;

#endregion

namespace ColloSys.FileUploadService.TextReader
{
    internal class CCollageReader : MultiLineTextReader<CLiner>
    {
        #region constructor

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public CCollageReader(FileScheduler file)
            : base(file, new FileReaderProperties())
        {
        }

        #endregion

        #region Read Next Row

        protected override TextFileRow<string[]> GetNextRow()
        {
            while (!HasEofReached())
            {
                string firstLine;
                string secondLine;
                string thirdLine;

                lock (InputFileStream)
                {
                    LineNo++;
                    firstLine = InputFileStream.ReadLine();
                    if ((firstLine == null) || (string.IsNullOrWhiteSpace(firstLine)))
                    {
                        Reader.Counter.AddIgnoredRecord(LineNo);
                        continue;
                    }

                    LineNo++;
                    secondLine = InputFileStream.ReadLine();
                    if ((secondLine == null) || (string.IsNullOrWhiteSpace(secondLine)))
                    {
                        Reader.Counter.AddIgnoredRecord(LineNo);
                        continue;
                    }

                    LineNo++;
                    thirdLine = InputFileStream.ReadLine();
                    if ((thirdLine == null) || (string.IsNullOrWhiteSpace(thirdLine)))
                    {
                        Reader.Counter.AddIgnoredRecord(LineNo);
                        continue;
                    }
                }

                try
                {
                    var scbIndex = firstLine.IndexOf("STANDARD CHARTERED BANK INDIA",
                                                     StringComparison.InvariantCulture);
                    if (scbIndex > 0)
                    {
                        Reader.Counter.AddIgnoredRecord(RowNo);
                        continue;
                    }

                    firstLine.Substring2(10, 25);
                    // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                    Convert.ToUInt64(secondLine.Substring2(1, 16));
                    Convert.ToUInt64(thirdLine.Substring2(22, 16));
                    // ReSharper restore ReturnValueOfPureMethodIsNotUsed

                    return new TextFileRow<string[]> { LineNo = LineNo, RowValue = new[] { firstLine, secondLine, thirdLine } };
                }
                catch
                {
                    // ReSharper disable RedundantJumpStatement
                    Reader.Counter.AddIgnoredRecord(LineNo);
                    continue;
                    // ReSharper restore RedundantJumpStatement
                }
            }

            return null;
        }

        #endregion

        #region Generate Record

        protected override CLiner GetRecord(string[] row)
        {
            var firstLine = row[0];
            var secondLine = row[1];
            var thirdLine = row[2];

            var liner = new CLiner
                {
                    CustomerName = firstLine.Substring2(10, 25),
                    Cycle = Convert.ToUInt16(firstLine.Substring2(35, 2)),
                    Block = firstLine.Substring2(47, 1),
                    AltBlock = firstLine.Substring2(49, 1),
                    CurrentBalance = Convert.ToDecimal(firstLine.Substring2(55, 13)),
                    LastPayAmount = Convert.ToDecimal(firstLine.Substring2(85, 9)),
                    Bucket0Due = Convert.ToDecimal(firstLine.Substring2(95, 12)),
                    Bucket3Due = Convert.ToDecimal(firstLine.Substring2(108, 12)),
                    Bucket6Due = Convert.ToDecimal(firstLine.Substring2(121, 12)),
                    AccountNo = Convert.ToUInt64(secondLine.Substring2(1, 16)).ToString(CultureInfo.InvariantCulture),
                    LastPayDate = Utilities.ParseNullableDateTime(secondLine.Substring2(86, 8), "dd/MM/yy"),
                    Bucket1Due = Convert.ToDecimal(secondLine.Substring2(95, 12)),
                    Bucket4Due = Convert.ToDecimal(secondLine.Substring2(108, 12)),
                    Bucket7Due = Convert.ToDecimal(secondLine.Substring2(121, 12)),
                    GlobalCustId = thirdLine.Substring2(22, 16),
                    CreditLimit = Utilities.ConvertToDecimal(thirdLine.Substring2(55, 13)),
                    Location = thirdLine.Substring2(42, 2),
                    CurrentDue = Convert.ToDecimal(thirdLine.Substring2(82, 12)),
                    Bucket2Due = Convert.ToDecimal(thirdLine.Substring2(95, 12)),
                    Bucket5Due = Convert.ToDecimal(thirdLine.Substring2(108, 12)),
                    TotalDue = Convert.ToDecimal(thirdLine.Substring2(121, 12))
                };

            if (liner.AccountNo.Length != 16)
            {
                throw new Exception("Invalid Account Number :" + liner.AccountNo);
            }

            return liner;
        }

        protected override void PopulateDefault(CLiner record)
        {
            record.FileDate = Reader.UploadedFile.FileDate.Date;
            record.Product = ScbEnums.Products.CC;
            record.Flag = ColloSysEnums.DelqFlag.O;
            record.DelqHistoryString += record.Bucket;
            record.AccountStatus = ColloSysEnums.DelqAccountStatus.Norm;
            PopulateComputed(record);
            //setZFlag(record);
        }

        private void PopulateComputed(CLiner record)
        {
            record.Bucket = getBucketForCLiner(record).ToString(CultureInfo.InvariantCulture);
            record.Bucket5Due = record.Bucket5Due + record.Bucket6Due + record.Bucket7Due;// bucket 5 due is sum of all above due
            record.BucketAmount = getBucketAmountForCLiner(record);

            // montly fields
            record.OutStandingBalance = record.CurrentBalance + record.UnbilledDue;
            record.TotalDue = record.CurrentDue;
            record.PeakBucket = record.Bucket;

            //set account z flag
            record.DelqHistoryString += record.Bucket;
            record.Flag = ColloSysEnums.DelqFlag.N;
            record.AccountStatus = ColloSysEnums.DelqAccountStatus.Norm;
            //SetAccountZFlag(record);
        }

        protected override bool IsRecordValid(CLiner record)
        {
            //var isValid = false;
            var cardNo = record.AccountNo.ToString(CultureInfo.InvariantCulture);
            var validChar = new List<char> { '9', '4', '5' };
            if (!validChar.Contains(cardNo[0]))
                return false;

            // if card number start from '41290537' will not consider for india because these card for bhutan.
            if (cardNo.StartsWith("41290537"))
                return false;

            // dummy accounts
            var zAccountCustId = new List<string> { "0999999900000810", "0999999900001210" };
            if (zAccountCustId.Contains(record.GlobalCustId))
                return false;

            return true;
        }

        #endregion

        #region Handle Unique Record

        protected override bool PerformUpdates(CLiner helper)
        {
            return true;
        }

        #endregion

        #region post-processing

        public override bool PostProcesing()
        {
            // check if this is last file in the list
            var doneFiles = Reader.GetDataLayer.GetDoneFile(Reader.UploadedFile.FileDetail.AliasName,
                                                            Reader.UploadedFile.FileDate.Date);
            if (doneFiles != Reader.UploadedFile.FileDetail.FileCount - 1)
                return true;

            // get data for yesterday
            _logger.Info("CCMS Liner : Post Processing : Started Fetching Previous Day Liner ");
            var prevDayLiner = GetPreviousDayLiner();
            _logger.Info("CCMS Liner : Post Processing : Previous Day Liner count :" + prevDayLiner.Count);

            // get data for today
            var entities = Reader.GetDataLayer.GetDataForDate<CLiner>(Reader.UploadedFile.FileDate.Date);
            entities.ForEach(x => TodayRecordList[x.AccountNo] = x);
            _logger.Info("CCMS Liner : Post Processing : Today Liner count :" + TodayRecordList.Count);

            // insert yesterday missing customers
            _logger.Info("CCMS Liner : Post Processing : Started Inserting Missing Entries ");
            InsertResolvedAccounts(prevDayLiner);


            //logger.Info("CCMS Liner : Post Processing : Started Updating flag from Open to Normalized account");
            //InsertNormalizedAccounts(prevDayLiner);

            // insert into info
            _logger.Info("CCMS Liner : Post Processing : Started Flag Computation ");
            SetGroupLevelComputedFields(prevDayLiner);

            // insert into info
            _logger.Info("CCMS Liner : Post Processing : Started Inserting Into CInfo ");
            InsertIntoCInfo();

            // insert Z Customer
            //_logger.Info("CCMS Liner : Post Processing : Started Making 'Z' by GlobalCustId ");
            //WriteoffBasedonGcustId(zaccounts);

            return true;
        }

        private Dictionary<string, CLiner> GetPreviousDayLiner()
        {
            var records = Reader.GetDataLayer
                                .GetDataForPreviousDay<CLiner>(Reader.UploadedFile.FileDetail.AliasName,
                                                               Reader.UploadedFile.FileDate.Date,
                                                               Reader.UploadedFile.FileDetail.FileCount);
            _logger.Info("CCMS Liner : Post Processing : Total customers for previous day  => " + records.Count);
            if (records.Count <= 0) return new Dictionary<string, CLiner>();

            var isNewMonth = records.First().FileDate.Month != Reader.UploadedFile.FileDate.Month;
            Dictionary<string, CLiner> previousDayLiner;
            if (isNewMonth)
            {
                previousDayLiner = records.Where(
                    x => x.Flag == ColloSysEnums.DelqFlag.O || x.Flag == ColloSysEnums.DelqFlag.N)
                                          .ToDictionary(x => x.AccountNo);
                _logger.Info("CCMS Liner : Post Processing : Remaining Customers due to month end  => " + previousDayLiner.Count);
            }
            else
            {
                previousDayLiner = records.ToDictionary(record => record.AccountNo);
            }

            return previousDayLiner;
        }

        private void InsertResolvedAccounts(Dictionary<string, CLiner> previousDayLiner)
        {
            var missingEntries = (from liner in previousDayLiner
                                  where !TodayRecordList.ContainsKey(liner.Key)
                                  select liner.Value)
                .ToList();

            foreach (var liner in missingEntries)
            {
                liner.ResetUniqueProperties();
                liner.Flag = (liner.Flag == ColloSysEnums.DelqFlag.Z) ? ColloSysEnums.DelqFlag.Z : ColloSysEnums.DelqFlag.R;
                liner.AccountStatus = ColloSysEnums.DelqAccountStatus.Norm;
                liner.Allocs = null;
                liner.FileDate = Reader.UploadedFile.FileDate;
                liner.FileScheduler = Reader.UploadedFile;
                liner.FileRowNo = 0;
                Reader.Counter.AddUploadRecord(0);
            }

            Reader.GetDataLayer.SaveOrUpdateData(missingEntries);
            _logger.Info("CCMS Liner : Post Processing : Missing Customer Count  => " + missingEntries.Count);

            missingEntries.ForEach(x => TodayRecordList[x.AccountNo] = x);
            _logger.Info("CCMS Liner : Post Processing : Today Records after adding missing customers  => " + TodayRecordList.Count);
        }
        
        private void InsertIntoCInfo()
        {
            var cinfos = Reader.GetDataLayer.GetTableData<CustomerInfo>().ToDictionary(x => x.AccountNo);

            var newAccounts = new List<CustomerInfo>();

            foreach (var kvpair in TodayRecordList)
            {
                var cinfo = cinfos.ContainsKey(kvpair.Key) ? cinfos[kvpair.Key] : new CustomerInfo();
                var cLiner = kvpair.Value;

                cinfo.AccountNo = cLiner.AccountNo;
                cinfo.GlobalCustId = cLiner.GlobalCustId;
                cinfo.CustomerName = cLiner.CustomerName;
                cinfo.Pincode = cLiner.Pincode;
                cinfo.Product = cLiner.Product;
                cinfo.IsInRecovery = (cinfo.Flag == ColloSysEnums.DelqFlag.Z);
                cinfo.ChargeofDate = cinfo.ChargeofDate ?? cLiner.FileDate;
                //cinfo.AccountStatus = cLiner.AccountStatus;
                cinfo.AllocStatus = cLiner.AllocStatus;
                //cinfo.AltBlock = cLiner.AltBlock;
                //cinfo.Block = cLiner.Block;
                cinfo.Bucket = cLiner.Bucket;
                //cinfo.Bucket0Due = cLiner.Bucket0Due;
                //cinfo.Bucket1Due = cLiner.Bucket1Due;
                //cinfo.Bucket2Due = cLiner.Bucket2Due;
                //cinfo.Bucket3Due = cLiner.Bucket3Due;
                //cinfo.Bucket4Due = cLiner.Bucket4Due;
                //cinfo.Bucket5Due = cLiner.Bucket5Due;
                //cinfo.Bucket6Due = cLiner.Bucket6Due;
                //cinfo.Bucket7Due = cLiner.Bucket7Due;
                //cinfo.BucketAmount = cLiner.BucketAmount;
                //cinfo.CreditLimit = cLiner.CreditLimit;
                //cinfo.CurrentBalance = cLiner.CurrentBalance;
                //cinfo.CurrentDue = cLiner.CurrentDue;
                cinfo.CustStatus = cLiner.CustStatus;
                //cinfo.CustTotalDue = cLiner.CustTotalDue;
                cinfo.Cycle = cLiner.Cycle;
                //cinfo.DelqHistoryString = cLiner.DelqHistoryString;
                //cinfo.FileDate = cLiner.FileDate;
                //cinfo.FileRowNo = cLiner.FileRowNo;
                //cinfo.FileScheduler = cLiner.FileScheduler;
                cinfo.Flag = cLiner.Flag;
                cinfo.GPincode = cLiner.GPincode;
                cinfo.IsReferred = cLiner.IsReferred;
                //cinfo.LastPayAmount = cLiner.LastPayAmount;
                //cinfo.LastPayDate = cLiner.LastPayDate;
                //cinfo.Location = cLiner.Location;
                cinfo.NoAllocResons = cLiner.NoAllocResons;
                //cinfo.OutStandingBalance = cLiner.OutStandingBalance;
                //cinfo.PeakBucket = cLiner.PeakBucket;
                cinfo.TotalDue = cLiner.TotalDue;
                //cinfo.UnbilledDue = cLiner.UnbilledDue;

                newAccounts.Add(cinfo);
            }

            _logger.Info("CCMS Liner : Post Processing : New Info Customer Count  => " + newAccounts.Count);
            Reader.GetDataLayer.SaveOrUpdateData(newAccounts);
        }



        private bool UpdateMonthlyFields(CLiner ydayRecord, CLiner todayRecord)
        {
            if (ydayRecord == null)
            {
                return false;
            }

            if (todayRecord.Cycle > ydayRecord.FileDate.Day && todayRecord.Cycle <= Reader.UploadedFile.FileDate.Day)
            {
                todayRecord.DelqHistoryString = ydayRecord.DelqHistoryString.Substring(1) + todayRecord.Bucket;
                todayRecord.TotalDue = todayRecord.CurrentDue;
                todayRecord.PeakBucket = todayRecord.Bucket;
            }
            else
            {
                todayRecord.DelqHistoryString = ydayRecord.DelqHistoryString;
                todayRecord.TotalDue = ydayRecord.TotalDue;
                todayRecord.PeakBucket = ydayRecord.PeakBucket;
            }

            if (todayRecord.Flag == ColloSysEnums.DelqFlag.N)
                todayRecord.Flag = ColloSysEnums.DelqFlag.O;

            return true;
        }

        private void SetGroupLevelComputedFields(Dictionary<string, CLiner> previousDayLiner)
        {
            var clinerList = new List<CLiner>();
            var unbilledRecordList = Reader.GetDataLayer.GetUnbilledAmounts(Reader.UploadedFile.FileDate.Date);
            var custTotalDueRecordList = Reader.GetDataLayer.GetCustTotalDue(Reader.UploadedFile.FileDate.Date);
            //var zAccountCustId = Reader.GetDataLayer.GetZGlobalCustId(Reader.UploadedFile.FileDate.Date);
            foreach (var kvpair in TodayRecordList)
            {
                bool hasChanged = false;
                var record = kvpair.Value;
                var unbilledDue = unbilledRecordList.GetValueOrDefault(record.AccountNo, 0);
                if (record.UnbilledDue != unbilledDue)
                {
                    record.UnbilledDue = unbilledDue;
                    hasChanged = true;
                }

                var custTotalDue = custTotalDueRecordList.GetValueOrDefault(record.GlobalCustId, 0);
                if (record.CustTotalDue != custTotalDue)
                {
                    record.CustTotalDue = custTotalDue;
                    hasChanged = true;
                }

                //if (zAccountCustId.Contains(record.GlobalCustId))
                //{
                //    record.Flag = ColloSysEnums.DelqFlag.Z;
                //    hasChanged = true;
                //}


                var yesterdayEntry = previousDayLiner.SingleOrDefault(x => x.Value.AccountNo == record.AccountNo).Value;
                var isUpdate = UpdateMonthlyFields(yesterdayEntry, record);

                if (hasChanged || isUpdate)
                    clinerList.Add(record);
            }

            Reader.GetDataLayer.SaveOrUpdateData(clinerList);
        }

        #endregion

        #region Helper

        private uint getBucketForCLiner(CLiner cLiner)
        {
            if (cLiner.Bucket7Due != 0)
                return 7;
            if (cLiner.Bucket6Due != 0)
                return 6;
            if (cLiner.Bucket5Due != 0)
                return 5;
            if (cLiner.Bucket4Due != 0)
                return 4;
            if (cLiner.Bucket3Due != 0)
                return 3;
            if (cLiner.Bucket2Due != 0)
                return 2;
            if (cLiner.Bucket1Due != 0)
                return 1;

            return 0;
        }

        private decimal getBucketAmountForCLiner(CLiner cLiner)
        {
            if (cLiner.Bucket5Due != 0)
                return cLiner.Bucket5Due;
            if (cLiner.Bucket4Due != 0)
                return cLiner.Bucket4Due;
            if (cLiner.Bucket3Due != 0)
                return cLiner.Bucket3Due;
            if (cLiner.Bucket2Due != 0)
                return cLiner.Bucket2Due;
            if (cLiner.Bucket1Due != 0)
                return cLiner.Bucket1Due;

            return 0;
        }

        #endregion
    }
}


//private void SetAccountZFlag(CLiner todayRecord)
//{
//    if (todayRecord.Flag == ColloSysEnums.DelqFlag.Z)
//        return;

//    var monthDay = Reader.UploadedFile.FileDate.Day;
//    if (todayRecord.Bucket == 5 && todayRecord.Cycle <= monthDay)
//    {
//        todayRecord.Flag = ColloSysEnums.DelqFlag.Z;
//        return;
//    }

//    if (todayRecord.Bucket == 3
//        && todayRecord.Cycle <= monthDay
//        && todayRecord.Block.ToUpperInvariant() == "K"
//        && todayRecord.TotalDue != 0)
//    {
//        todayRecord.Flag = ColloSysEnums.DelqFlag.Z;
//    }
//}

//private void setZFlag(CLiner liner)
//{
//    if (liner.Flag == ColloSysEnums.DelqFlag.Z)
//        return;

//    var monthDay = Reader.UploadedFile.FileDate.Day;
//    if (liner.Bucket == 5 && liner.Cycle <= monthDay)
//    {
//        liner.Flag = ColloSysEnums.DelqFlag.Z;
//        return;
//    }

//    if (liner.Bucket == 3
//        && liner.Cycle <= monthDay
//        && liner.Block.ToUpperInvariant() == "K"
//        && liner.TotalDue != 0)
//    {
//        liner.Flag = ColloSysEnums.DelqFlag.Z;
//    }
//}

//private void ComputeFlag(CLiner ydayRecord, CLiner todayRecord, HashSet<string> zAccountCustId)
//        {
//            //global cust id
//            if (zAccountCustId.Contains(todayRecord.GlobalCustId))
//            {
//                todayRecord.Flag = ColloSysEnums.DelqFlag.Z;
//                return;
//            }

//            if (todayRecord.Flag == ColloSysEnums.DelqFlag.Z)
//            {
//                zAccountCustId.Add(todayRecord.GlobalCustId);
//                return;
//            }

//            // open
//            if (ydayRecord != null)
//            {
//                if (ydayRecord.Flag == ColloSysEnums.DelqFlag.Z)
//                {
//                    todayRecord.Flag = ColloSysEnums.DelqFlag.Z;
//                    zAccountCustId.Add(todayRecord.GlobalCustId);
//                    return;
//                }
//                todayRecord.Flag = ColloSysEnums.DelqFlag.O;
//            }

//            // curbal zero
//            if (todayRecord.CurrentBalance == 0)
//            {
//                todayRecord.Flag = ColloSysEnums.DelqFlag.R;
//            }

//            // bucket 5 & cycle
//            var monthDay = Reader.UploadedFile.FileDate.Day;
//            if (todayRecord.Bucket == 5 && todayRecord.Cycle <= monthDay)
//            {
//                todayRecord.Flag = ColloSysEnums.DelqFlag.Z;
//                zAccountCustId.Add(todayRecord.GlobalCustId);
//            }

//            // K block
//            if (todayRecord.Bucket == 3
//                && todayRecord.Cycle <= monthDay
//                && todayRecord.Block.ToUpperInvariant() == "K"
//                && todayRecord.TotalDue != 0)
//            {
//                todayRecord.Flag = ColloSysEnums.DelqFlag.Z;
//                zAccountCustId.Add(todayRecord.GlobalCustId);
//            }
//        }



//private void WriteoffBasedonGcustId(HashSet<string> zAccountCustId)
//{
//    var totalDue = Reader.GetDataLayer.GetCustTotalDue(Reader.UploadedFile.FileDate.Date);
//    var changedAccounts = new List<CLiner>();
//    foreach (var kvpair in TodayRecordList)
//    {
//        var record = kvpair.Value;
//        if (record.Flag != ColloSysEnums.DelqFlag.Z && zAccountCustId.Contains(record.GlobalCustId))
//        {
//            record.Flag = ColloSysEnums.DelqFlag.Z;
//            changedAccounts.Add(record);
//        }

//        if (!totalDue.ContainsKey(record.GlobalCustId)) continue;
//        var totbal = totalDue[record.GlobalCustId];
//        if (record.CustTotalDue != totbal)
//        {
//            changedAccounts.Add(record);
//        }
//    }

//    Reader.GetDataLayer.SaveOrUpdateData(changedAccounts);
//}


//private void InsertNormalizedAccounts(Dictionary<string, CLiner> previousDayLiner)
//        {
//            var todayNewEntries = (from liner in TodayRecordList
//                                   where !previousDayLiner.ContainsKey(liner.Key) && liner.Value.Flag != ColloSysEnums.DelqFlag.Z
//                                   select liner.Value)
//                .ToList();

//            foreach (var liner in todayNewEntries)
//            {
//                liner.Flag = ColloSysEnums.DelqFlag.N;
//                setZFlag(liner);
//            }

//            Reader.GetDataLayer.SaveOrUpdateData(todayNewEntries);
//        }


