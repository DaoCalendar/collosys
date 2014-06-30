using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.RecordManager;
using ColloSys.FileUploadService.TextReader;
using ColloSys.Shared.SharedUtils;
using NHibernate.Hql.Ast.ANTLR;
using NPOI.SS.Formula.Functions;

namespace ColloSys.FileUploaderService.AliasTextReader
{
    public class CCollageRecordCreator : TextRecordCreator<CLiner>
    {
        string _firstLine = string.Empty;
        string _secondLine = string.Empty;
        string _thirdLine = string.Empty;

        public override bool CreateRecord(out CLiner obj)
        {

            if (!CheckValidRecord(InpuStreamReader))
            {
                obj = null;
                return false;
            }
            var firstLine = _firstLine;
            var secondLine = _secondLine;
            var thirdLine = _thirdLine;
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
                LastPayDate = Shared.SharedUtils.Utilities.ParseNullableDateTime(secondLine.Substring2(86, 8), "dd/MM/yy"),
                Bucket1Due = Convert.ToDecimal(secondLine.Substring2(95, 12)),
                Bucket4Due = Convert.ToDecimal(secondLine.Substring2(108, 12)),
                Bucket7Due = Convert.ToDecimal(secondLine.Substring2(121, 12)),
                GlobalCustId = thirdLine.Substring2(22, 16),
                CreditLimit = Shared.SharedUtils.Utilities.ConvertToDecimal(thirdLine.Substring2(55, 13)),
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
            PopulateComputed(liner);
            PopulateDefault(liner);
            if (!IsRecordValid(liner))
            {
                Counter.IncrementIgnoreRecord();
                obj = null;
                return false;
            }
            Counter.IncrementValidRecords();
            obj = liner;
            return true;
        }

        protected void PopulateDefault(CLiner record)
        {
            record.FileDate = FileScheduler.FileDate.Date;
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

        protected bool IsRecordValid(CLiner record)
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

        public  CLiner GetPreviousDayEntity(CLiner entity)
        {
            return YesterdayRecords.SingleOrDefault(x => x.AccountNo == entity.AccountNo);
        }

       
        //private Dictionary<string, CLiner> GetPreviousDayLiner()
        //{
        //    var records = DbLayer
        //                        .GetDataForPreviousDay<CLiner>(FileScheduler.FileDetail.AliasName,
        //                                                       FileScheduler.FileDate.Date,
        //                                                       FileScheduler.FileDetail.FileCount);
        //    Logger.Info("CCMS Liner : Post Processing : Total customers for previous day  => " + records.Count);
        //    if (records.Count <= 0) return new Dictionary<string, CLiner>();

        //    var isNewMonth = records.First().FileDate.Month != FileScheduler.FileDate.Month;
        //    Dictionary<string, CLiner> previousDayLiner;
        //    if (isNewMonth)
        //    {
        //        previousDayLiner = records.Where(
        //            x => x.Flag == ColloSysEnums.DelqFlag.O || x.Flag == ColloSysEnums.DelqFlag.N)
        //                                  .ToDictionary(x => x.AccountNo);
        //        Logger.Info("CCMS Liner : Post Processing : Remaining Customers due to month end  => " + previousDayLiner.Count);
        //    }
        //    else
        //    {
        //        previousDayLiner = records.ToDictionary(record => record.AccountNo);
        //    }

        //    return previousDayLiner;
        //}
        private bool CheckValidRecord(StreamReader reader)
        {
            _firstLine = reader.ReadLine();
            if ((_firstLine == null) || (string.IsNullOrWhiteSpace(_firstLine)))
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }

            Counter.IncrementLineNo();
            _secondLine = reader.ReadLine();
            if ((_secondLine == null) || (string.IsNullOrWhiteSpace(_secondLine)))
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }

            Counter.IncrementLineNo();
            _thirdLine = reader.ReadLine();
            if ((_thirdLine == null) || (string.IsNullOrWhiteSpace(_thirdLine)))
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }

            try
            {
                var scbIndex = _firstLine.IndexOf("STANDARD CHARTERED BANK INDIA",
                                                    StringComparison.InvariantCulture);
                if (scbIndex > 0)
                {
                    Counter.IncrementIgnoreRecord();
                    return false;
                }

                _firstLine.Substring2(10, 25);
                // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                Convert.ToUInt64(_secondLine.Substring2(1, 16));
                Convert.ToUInt64(_thirdLine.Substring2(22, 16));
                // ReSharper restore ReturnValueOfPureMethodIsNotUsed

                return true;
            }
            catch (Exception)
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }
        }
        public override CLiner GetRecordForUpdate()
        {
            throw new NotImplementedException();
        }

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
