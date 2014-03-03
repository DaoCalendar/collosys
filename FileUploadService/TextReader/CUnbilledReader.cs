using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.Domain;
using System;
using ColloSys.FileUploadService.Interfaces;
using ColloSys.Shared.SharedUtils;

namespace ColloSys.FileUploadService.TextReader
{
    internal class CUnbilledReader : SingleLineTextReader<CUnbilled>
    {
        #region constructor

        private static FileReaderProperties GetFileReaderProperties()
        {
            return new FileReaderProperties
            {
                //HasFileDateInsideFile = false
            };
        }

        public CUnbilledReader(FileScheduler file)
            : base(file, GetFileReaderProperties())
        {

        }

        #endregion

        #region Read Next Row
        protected override TextFileRow<string> GetNextRow()
        {
            while (!HasEofReached())
            {
                string currentLine;

                lock (InputFileStream)
                {
                    RowNo++;
                    currentLine = InputFileStream.ReadLine();
                }

                if ((currentLine == null) || (string.IsNullOrWhiteSpace(currentLine)))
                {
                    Reader.Counter.AddIgnoredRecord(RowNo);
                    continue;
                }

                try
                {
                    var scbIndex = currentLine.IndexOf("STANDARD CHARTERED BANK INDIA",
                                                      StringComparison.InvariantCulture);
                    if (scbIndex > 0)
                    {
                        //var filedatestring = currentLine.Substring2((uint)(100 + scbIndex), 8);
                        //Reader.UploadedFile.FileDate = Utilities.ParseDateTime(filedatestring, "dd/MM/yy");
                        Reader.Counter.AddIgnoredRecord(RowNo);
                        continue;
                    }

                    return new TextFileRow<string> { LineNo = RowNo, RowValue = currentLine };
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch (Exception)
                // ReSharper restore EmptyGeneralCatchClause
                {
                    Reader.Counter.AddIgnoredRecord(LineNo);
                }
            }

            return null;
        }

        #endregion

        #region Handle Unique Record

        protected override bool PerformUpdates(CUnbilled record)
        {
            return false;
        }

        #endregion

        #region Get Record
        protected override CUnbilled GetRecord(string row)
        {
            var currentLine = row;

            var unbilled = new CUnbilled
                {
                    AccountNo = Convert.ToUInt64(currentLine.Substring2(1, 16)).ToString(CultureInfo.InvariantCulture),
                    CustomerName = currentLine.Substring2(18, 15),
                    UnbilledAmount = Convert.ToDecimal(currentLine.Substring2(105, 15)),
                    StartDate = Utilities.ParseDateTime(currentLine.Substring2(34, 6), "ddMMyy"),

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

            return unbilled;
        }

        protected override void PopulateDefault(CUnbilled record)
        {

        }

        protected override bool IsRecordValid(CUnbilled record)
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
        #endregion

    }
}
