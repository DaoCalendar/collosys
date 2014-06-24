using System;
using System.Globalization;
using System.IO;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.RecordManager;
using ColloSys.Shared.SharedUtils;

namespace ColloSys.FileUploaderService.AliasTextReader
{
    public class VmtRecordCreator: TextRecordCreator<Payment>
    {
        private StreamReader _inpuStreamReader;

        string _firstLine = string.Empty;
        string _secondLine = string.Empty;

        public VmtRecordCreator()
        {
           
        }

        public override bool CreateRecord(out Payment payment)
        {
            var currentLine = InpuStreamReader.ReadLine();
            if (!CheckValidRecord(currentLine))
            {
                payment = null;
                return false;
            }

            try
            {
                var firstLine =_firstLine;
                var secondLine = _secondLine;

                var payments = new Payment
                {
                    AccountNo = Convert.ToUInt64(firstLine.Substring2(7, 16).Trim()).ToString(CultureInfo.InvariantCulture),
                    TransAmount = Convert.ToDecimal(firstLine.Substring2(47, 11)),
                    TransDesc = secondLine.Substring2(46, 13).Trim()
                };

                if (payments.AccountNo.ToString(CultureInfo.InvariantCulture).Length != 16)
                {
                    throw new Exception("Invalid Account Number :" + payments.AccountNo);
                }
                PopulateDefault(payments);
                if (!IsRecordValid(payments))
                {
                    payment = payments;
                    return false;
                }

                Counter.IncrementValidRecords();
                payment = payments;
            }
            catch (Exception)
            {
                payment = null;
                Counter.IncrementIgnoreRecord();
                return false;
            }
            
            return true;

        }

        public override Payment GetRecordForUpdate()
        {
            throw new NotImplementedException();
        }

        private bool CheckValidRecord(string currentLine)
        {
          
            if ((currentLine == null) || (string.IsNullOrWhiteSpace(currentLine)))
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }

            try
            {
                if (currentLine.Contains("PAN:"))
                {
                    _firstLine = currentLine;
                    Counter.IncrementIgnoreRecord();
                    return false;
                }

                if (currentLine.Contains("MERCHANT:"))
                {
                    _secondLine = currentLine;
                    Counter.IncrementIgnoreRecord();
                    return false;
                }

                if (currentLine.Contains("MCC:"))
                {
                    var mcc = Convert.ToInt32(currentLine.Substring2(99, 5).Trim());

                    if (mcc == 6012 || mcc == 6051)
                    {
                        return true;
                        //return new TextFileRow<string[]>
                        //{
                        //    LineNo = LineNo,
                        //    RowValue = new[] { firstLine, secondLine }
                        //};
                    }
                    Counter.IncrementIgnoreRecord();
                }
                return false;
            }
            catch (Exception)
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }
        }

        protected  bool IsRecordValid(Payment record)
        {
            if (record.AccountNo.ToString(CultureInfo.InvariantCulture).Length != 16)
            {
                Counter.IncrementIgnoreRecord();
                //_log.Info(string.Format("Card No : {0} has length {1} is not valid", record.AccountNo, record.AccountNo.ToString(CultureInfo.InvariantCulture).Length));
                return false;
            }

            if (record.TransAmount <= 0)
            {
                Counter.IncrementIgnoreRecord();
                //_log.Info(string.Format("Card No : {0} has TransAmount {1} is not valid", record.AccountNo, record.TransAmount));
                return false;
            }

            if (record.TransDesc.ToUpper().IndexOf("VISA", StringComparison.InvariantCulture) < 0)
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }

            return true;
        }

        protected  void PopulateDefault(Payment record)
        {
            record.Status = ColloSysEnums.ApproveStatus.NotApplicable;
            record.BillStatus = ColloSysEnums.BillStatus.Unbilled;
            record.IsExcluded = false;
            record.ExcludeReason = string.Empty;
            record.TransCode = 20;
            record.IsDebit = true;
            record.Products = ScbEnums.Products.CC;

            record.TransDate = FileScheduler.FileDate.Date;
            record.BillDate = null;
            record.FileDate = FileScheduler.FileDate.Date;

            if (string.IsNullOrWhiteSpace(record.TransDesc))
                record.TransDesc = "NotProvided";
        }
    }
}
