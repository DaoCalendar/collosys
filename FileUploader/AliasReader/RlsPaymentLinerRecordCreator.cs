using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using NLog;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.AliasReader
{
   public class RlsPaymentLinerRecordCreator
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly FileScheduler _uploadedFile;

          public RlsPaymentLinerRecordCreator(FileScheduler fileScheduler)
        {
              _uploadedFile = fileScheduler;
        }

        public bool ComputedSetter(Payment record )
        {
            record.FileDate = _uploadedFile.FileDate;

            if (_uploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.R_PAYMENT_LINER)
            {
                IList<int> paymentcodes = new List<int> { 179, 201, 203, 891 };
                record.IsDebit = paymentcodes.Contains(record.TransCode);
                var amountdiff = record.DebitAmount - (record.CreditAmount.HasValue ? record.CreditAmount.Value : 0);
                record.TransAmount = amountdiff * (amountdiff < 0 ? -1 : 1);
                return true;
            }
            else
            {
                record.IsDebit = (record.TransAmount > 0);
            }
            return true;
        }

        public bool IsRecordValid(Payment record)
        {
            if (record.TransDate < new DateTime(2000, 1, 1))
            {
            
                //var fileMapping = _uploadedFile.FileDetail.FileMappings
                //                        .Single(m => m.ActualTable == record.GetType().Name
                //                            && m.ActualColumn == ReflectionHelper.GetMemberName<Payment>(x => x.TransDate));

                _log.Info(string.Format("Account No : {0} has TransDate {1} is not valid", record.AccountNo, record.TransDate));
             //errorDescription = GetErrorDescription(fileMapping, "Transaction Date is not valid");
                return false;
            }
            return true;
        }

        public bool PerformUpdates(Payment record)
        {
            return false;
        }

        public bool CheckBasicField(Payment record, IExcelReader reader)
        {
            string loanNo;
            var aliseName = _uploadedFile.FileDetail.AliasName;

            if (aliseName == ColloSysEnums.FileAliasName.R_PAYMENT_WO_AEB ||
                                aliseName == ColloSysEnums.FileAliasName.R_PAYMENT_WO_PLPC)
            {
                loanNo = reader.GetValue(1); // dr[1].ToString();
            }
            else
            {
                loanNo = reader.GetValue(3); //dr[3].ToString();
            }

            ulong loanNumber;
            if (!ulong.TryParse(loanNo, out loanNumber) || (loanNumber.ToString(CultureInfo.InvariantCulture).Length < 3))
            {
                _log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", loanNo));
                return false;
            }
            if (aliseName == ColloSysEnums.FileAliasName.R_PAYMENT_WO_AEB ||
                             aliseName == ColloSysEnums.FileAliasName.R_PAYMENT_WO_PLPC)
            {
                loanNumber.ToString("D8");
            }
            else
            {
                loanNumber.ToString("D8");
            }
            return true;
        }
    }
}
