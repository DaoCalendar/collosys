#region references

using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploadService.Interfaces;
using NLog;

#endregion

namespace ColloSys.FileUploadService.ExcelReader
{
    public class EPaymentReader : SingleTableExcelReader<Payment>, IExcelFile<Payment>
    {
        #region constructor

        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly IList<string> _ePaymentExcludeCodes;
        private readonly List<string> _eWriteoffAccounts;

        private static FileReaderProperties GetFileReaderProperties()
        {
            return new FileReaderProperties();
        }

        public EPaymentReader(FileScheduler file)
            : base(file, GetFileReaderProperties())
        {
            _ePaymentExcludeCodes = Reader.GetDataLayer.GetValuesFromKey(ColloSysEnums.Activities.FileUploader, "EPaymentExcludeCode");
            _eWriteoffAccounts = Reader.GetDataLayer.GetAccountNosForDate<EWriteoff>(Reader.UploadedFile.FileDate.Date);
        }

        #endregion

        #region overrid methods

        public override bool PopulateComputedValue(Payment record, out string errorDescription)
        {
            // file Date
            record.FileDate = Reader.UploadedFile.FileDate;

            // is payment
            if (Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.E_PAYMENT_LINER)
            {
                record.IsDebit = (record.DebitAmount > 0);
            }

            var shouldBeExclude = _ePaymentExcludeCodes.Contains(string.Format("{0}@{1}", record.TransCode, record.TransDesc.Trim()));
            record.IsExcluded = shouldBeExclude;
            record.ExcludeReason = string.Format("TransCode : {0}, and TransDesc : {1}", record.TransCode, record.TransDesc);

            errorDescription = string.Empty;
            return true;
        }

        public override bool PerformUpdates(Payment record)
        {
            return false;
        }

        public override bool CheckBasicField(DataRow dr)
        {
            ulong loanNumber;
            if (!ulong.TryParse(dr[1].ToString(), out loanNumber) || (loanNumber.ToString(CultureInfo.InvariantCulture).Length < 5))
            {
                _log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", dr[1]));
                return false;
            }
            dr[1] = loanNumber.ToString("D11");

            // Exclude Payment record based on some code and narration
            if (Reader.UploadedFile.FileDetail.AliasName == ColloSysEnums.FileAliasName.E_PAYMENT_LINER)
            {
                var narr = dr[10].ToString().Trim() + dr[11].ToString().Trim() +
                           dr[12].ToString().Trim();
                var shouldBeExclude =
                    _ePaymentExcludeCodes.Contains(string.Format("{0}@{1}", dr[2].ToString().Trim(), narr));
                if (shouldBeExclude)
                {
                    _log.Debug(string.Format("Payment of Account No {0} is Excluded Because TransCode : {1}, and TransDesc : {2}",
                                                    dr[1], dr[2], narr));
                    return false;
                }

                if (_eWriteoffAccounts.Contains(dr[1]))
                {
                    _log.Debug(string.Format("Payment of Account No {0} is Excluded Because It is Writeoff Account",
                                             dr[1]));
                    return false;
                }
            }

            return true;
        }

        public override bool IsRecordValid(Payment record, out string errorDescription)
        {
            if (record.TransDate.Month != Reader.UploadedFile.FileDate.Month)
            {
                _log.Debug(string.Format("Payment of Account No {0} is not valid, " +
                                         "Because Transaction month is not match with FileDate month",
                                         record.AccountNo));
                errorDescription = string.Empty;
                return false;
            }

            errorDescription = string.Empty;
            return true;
        }

        #endregion
    }
}
