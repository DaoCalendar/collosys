using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;
using ColloSys.FileUploadService.TextReader;
using ColloSys.Shared.SharedUtils;

namespace ColloSys.FileUploaderService.AliasTextReader
{
    public class CUnbilledRecordCreator : TextRecordCreator<CUnbilled>
    {

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
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            


          
        }

        private bool CheckValidRecord(string reader)
        {
            string currentLine;


            //Counter.IncrementLineNo();
            currentLine = reader;


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
                    //var filedatestring = currentLine.Substring2((uint)(100 + scbIndex), 8);
                    //Reader.UploadedFile.FileDate = Utilities.ParseDateTime(filedatestring, "dd/MM/yy");
                    Counter.IncrementIgnoreRecord();
                    return false;
                }

                return true;
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }
        }

        protected bool IsRecordValid(CUnbilled record)
        {
            var cardNo = record.AccountNo.ToString(CultureInfo.InvariantCulture);
            var validChar = new List<char> { '9', '4', '5' };
            if (!validChar.Contains(cardNo[0]))
                return false;

            //var dummyAc = new List<ulong> { 0999999900000810, 0999999900001210 };
            //if (dummyAc.Contains(record.CardNo))
            //    isValid = false;

            return true;
        }

        public override CUnbilled GetRecordForUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
