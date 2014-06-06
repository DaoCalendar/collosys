#region references

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploadService.Interfaces;
using ColloSys.Shared.SharedUtils;
using NLog;

#endregion

namespace ColloSys.FileUploadService.ExcelReader
{
    public class RPaymentReader : SingleTableExcelReader<Payment>, IExcelFile<Payment>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region constructor

        private static FileReaderProperties GetFileReaderProperties()
        {
            return new FileReaderProperties
            {
                //HasFileDateInsideFile = false,
                CsvDelimiter = "\t"
            };
        }

        public RPaymentReader(FileScheduler file)
            : base(file, GetFileReaderProperties())
        {

        }

        #endregion

        #region overrid methods

        public override bool PopulateComputedValue(Payment record, out string errorDescription)
        {
            // file date
            record.FileDate = Reader.UploadedFile.FileDate;
            errorDescription = string.Empty;

            if (Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.R_PAYMENT_LINER)
            {
                IList<int> paymentcodes = new List<int> { 179, 201, 203, 891 };
                record.IsDebit = paymentcodes.Contains(record.TransCode);
                var amountdiff = record.DebitAmount - (record.CreditAmount.HasValue ? record.CreditAmount.Value : 0);
                record.TransAmount = amountdiff * (amountdiff < 0 ? -1 : 1);
            }
            else
            {
                //record.IsDebit = (record.DebitAmount > 0);
                //record.TransAmount = record.IsDebit ? record.DebitAmount : record.CreditAmount;
                record.IsDebit = (record.TransAmount > 0);
            }
            return true;
        }

        public override bool IsRecordValid(Payment record, out string errorDescription)
        {
            //var modelProp = new PropertyReflection<RPayment>();

            if (record.TransDate < new DateTime(2000, 1, 1))
            {
                //var fileMapping = FileMappingList.Single(m => m.ActualTable == record.GetType().Name
                //                            && m.ActualColumn == modelProp.GetName(r => r.TransDate));

                var fileMapping = Reader.UploadedFile.FileDetail.FileMappings
                                        .Single(m => m.ActualTable == record.GetType().Name
                                            && m.ActualColumn == record.GetMemberName<Payment>(x => x.TransDate));

                _log.Info(string.Format("Account No : {0} has TransDate {1} is not valid", record.AccountNo, record.TransDate));
                errorDescription = GetErrorDescription(fileMapping, "Transaction Date is not valid");
                return false;
            }

            errorDescription = string.Empty;
            return true;
        }

        public override bool PerformUpdates(Payment record)
        {
            return false;
        }

        public override bool CheckBasicField(DataRow dr)
        {
            string loanNo;
            var aliseName = Reader.UploadedFile.FileDetail.AliasName;

            if (aliseName == ColloSysEnums.FileAliasName.R_PAYMENT_WO_AEB ||
                                aliseName == ColloSysEnums.FileAliasName.R_PAYMENT_WO_PLPC)
            {
                loanNo = dr[1].ToString();
            }
            else
            {
                loanNo = dr[3].ToString();
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
                dr[1] = loanNumber.ToString("D8");
            }
            else
            {
                dr[3] = loanNumber.ToString("D8");
            }

            return true;
        }

        #endregion
    }
}