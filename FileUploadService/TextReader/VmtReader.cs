#region references

using System;
using System.Globalization;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.FileUploadService.Interfaces;
using ColloSys.Shared.SharedUtils;

#endregion

namespace ColloSys.FileUploadService.TextReader
{
    internal class VmtReader : MultiLineTextReader<Payment>
    {
        #region constructor
        //private static FileReaderProperties GetFileReaderProperties()
        //{
        //    return new FileReaderProperties
        //    {
        //        HasFileDateInsideFile = true
        //    };
        //}

        public VmtReader(FileScheduler file)
            : base(file, new FileReaderProperties())
        {
        }

        #endregion

        #region Read Next Row
        protected override TextFileRow<string[]> GetNextRow()
        {
            lock (InputFileStream)
            {
                string firstLine = string.Empty;
                string secondLine = string.Empty;

                while (!HasEofReached())
                {
                    try
                    {
                        LineNo++;
                        string currentLine = InputFileStream.ReadLine();

                        if ((currentLine == null) || (string.IsNullOrWhiteSpace(currentLine)))
                        {
                            Reader.Counter.AddIgnoredRecord(LineNo);
                            continue;
                        }

                        if (currentLine.Contains("PAN:"))
                        {
                            firstLine = currentLine;
                            Reader.Counter.AddIgnoredRecord(LineNo);
                            continue;
                        }

                        if (currentLine.Contains("MERCHANT:"))
                        {
                            secondLine = currentLine;
                            Reader.Counter.AddIgnoredRecord(LineNo);
                            continue;
                        }

                        if (currentLine.Contains("MCC:"))
                        {
                            var mcc = Convert.ToInt32(currentLine.Substring2(99, 5).Trim());

                            if (mcc == 6012 || mcc == 6051)
                            {
                                return new TextFileRow<string[]>
                                    {
                                        LineNo = LineNo,
                                        RowValue = new[] { firstLine, secondLine }
                                    };
                            }

                            Reader.Counter.AddIgnoredRecord(LineNo);
                        }
                    }
                    // ReSharper disable EmptyGeneralCatchClause
                    catch
                    // ReSharper restore EmptyGeneralCatchClause
                    {
                        Reader.Counter.AddIgnoredRecord(LineNo);
                    }
                }
            }

            return null;
        }

        #endregion

        #region Handle Unique Record

        protected override bool PerformUpdates(Payment record)
        {
            return false;
        }

        #endregion

        #region Get Record
        protected override Payment GetRecord(string[] row)
        {
            var firstLine = row[0];
            var secondLine = row[1];

            var payment = new Payment
                {
                    AccountNo = Convert.ToUInt64(firstLine.Substring2(7, 16).Trim()).ToString(CultureInfo.InvariantCulture),
                    TransAmount = Convert.ToDecimal(firstLine.Substring2(47, 11)),
                    TransDesc = secondLine.Substring2(46, 13).Trim()
                };

            if (payment.AccountNo.ToString(CultureInfo.InvariantCulture).Length != 16)
            {
                throw new Exception("Invalid Account Number :" + payment.AccountNo);
            }

            return payment;
        }

        protected override void PopulateDefault(Payment record)
        {
            record.Status = ColloSysEnums.ApproveStatus.NotApplicable;
            record.BillStatus = ColloSysEnums.BillStatus.Unbilled;
            record.IsExcluded = false;
            record.ExcludeReason = string.Empty;
            record.TransCode = 20;
            record.IsDebit = true;
            record.Products = ScbEnums.Products.CC;

            record.TransDate = Reader.UploadedFile.FileDate.Date;
            record.BillDate = null;
            record.FileDate = Reader.UploadedFile.FileDate.Date;

            if (string.IsNullOrWhiteSpace(record.TransDesc))
                record.TransDesc = "NotProvided";
        }

        protected override bool IsRecordValid(Payment record)
        {
            if (record.AccountNo.ToString(CultureInfo.InvariantCulture).Length != 16)
            {
                //_log.Info(string.Format("Card No : {0} has length {1} is not valid", record.AccountNo, record.AccountNo.ToString(CultureInfo.InvariantCulture).Length));
                return false;
            }

            if (record.TransAmount <= 0)
            {
                //_log.Info(string.Format("Card No : {0} has TransAmount {1} is not valid", record.AccountNo, record.TransAmount));
                return false;
            }

            if (record.TransDesc.ToUpper().IndexOf("VISA", StringComparison.InvariantCulture) < 0)
                return false;

            return true;
        }

        #endregion
    }
}
