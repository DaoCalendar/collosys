using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;
using ColloSys.Shared.SharedUtils;

namespace ColloSys.FileUploaderService.AliasTextReader
{
    public class CUnbilledRecordCreator : TextRecordCreator<CUnbilled>
    {
        public readonly Dictionary<string, decimal> UnbilledAmount = new Dictionary<string, decimal>(); 

        public override bool CreateRecord(out CUnbilled obj)
        {
            var currentLine = InpuStreamReader.ReadLine();

            if (!CheckValidRecord(currentLine))
            {
                obj = null;
                return false;
            }
            try
            {
                var unbilled = new CUnbilled
                {
                    AccountNo = Convert.ToUInt64(currentLine.Substring2(1, 16)).ToString(CultureInfo.InvariantCulture),
                    CustomerName = currentLine.Substring2(18, 15),
                    UnbilledAmount = Convert.ToDecimal(currentLine.Substring2(105, 15)),
                    StartDate = Shared.SharedUtils.Utilities.ParseDateTime(currentLine.Substring2(34, 6), "ddMMyy"),

                    OriginalTenure = Convert.ToUInt32(currentLine.Substring2(48, 7)),
                    //EndDate = StartDate //Utilities.ParseDateTime(currentLine.Substring2(41, 6), "ddMMyy"),
                    RemainingTenure = Convert.ToUInt32(currentLine.Substring2(55, 7)),
                    InterestPct = Convert.ToDecimal(currentLine.Substring2(62, 8)),
                    LoanAmount = Convert.ToDecimal(currentLine.Substring2(73, 15)),
                    BilledAmount = Convert.ToDecimal(currentLine.Substring2(88, 15)),
                    BilledInterest = Convert.ToDecimal(currentLine.Substring2(118, 14))

                };
                unbilled.EndDate = unbilled.StartDate.AddMonths((int)unbilled.OriginalTenure);

                if (unbilled.AccountNo.ToString(CultureInfo.InvariantCulture).Length != 16)
                {
                    throw new Exception("Invalid Account Number :" + unbilled.AccountNo);
                }

                if (!IsRecordValid(unbilled))
                {
                    obj = null;
                    return false;
                }

                Counter.IncrementValidRecords();
                obj = unbilled;
                AddRecordToList(unbilled);
                return false;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        private bool CheckValidRecord(string reader)
        {
            var currentLine = reader;
            if ((currentLine == null) || (string.IsNullOrWhiteSpace(currentLine)))
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }

            try
            {
                var scbIndex = currentLine.IndexOf("STANDARD CHARTERED BANK INDIA",
                                                  StringComparison.InvariantCulture);
                if (scbIndex > 0)
                {
                    Counter.IncrementIgnoreRecord();
                    return false;
                }

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert.ToDecimal(currentLine.Substring2(1, 16));
                return true;
            }
            catch (Exception)
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }
        }

        private bool IsRecordValid(CUnbilled record)
        {
            var cardNo = record.AccountNo.ToString(CultureInfo.InvariantCulture);
            var validChar = new List<char> { '9', '4', '5' };
            return (validChar.Contains(cardNo[0]));
        }

        public override CUnbilled GetRecordForUpdate()
        {
            throw new NotImplementedException();
        }

        private void AddRecordToList(CUnbilled record)
        {
            if (UnbilledAmount.ContainsKey(record.AccountNo))
            {
                UnbilledAmount[record.AccountNo] += record.UnbilledAmount;
            }
            else
            {
                UnbilledAmount[record.AccountNo] = record.UnbilledAmount;
            }
        }
    }
}
