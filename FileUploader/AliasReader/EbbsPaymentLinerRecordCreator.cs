using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using NLog;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.AliasReader
{
    class EbbsPaymentLinerRecordCreator : IAliasRecordCreator<Payment>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly FileScheduler _uploadedFile;
        private readonly List<string> _ePaymentExcludeCodes;
        private readonly List<string> _eWriteoffAccounts;

        public EbbsPaymentLinerRecordCreator(FileScheduler fileShedular, List<string> ePaymentExcludeCodes, List<string> eWriteoffAccounts)
        {
            _ePaymentExcludeCodes = ePaymentExcludeCodes;
            _eWriteoffAccounts = eWriteoffAccounts;
            _uploadedFile = fileShedular;
        }
        public bool ComputedSetter(Payment record, IExcelReader reader, ICounter counter)
        {
            try
            {
                record.FileDate = _uploadedFile.FileDate;
                record.IsDebit = (record.DebitAmount > 0);
                var shouldBeExclude = _ePaymentExcludeCodes.Contains(string.Format("{0}@{1}", record.TransCode, record.TransDesc.Trim()));
                record.IsExcluded = shouldBeExclude;
                record.ExcludeReason = string.Format("TransCode : {0}, and TransDesc : {1}", record.TransCode, record.TransDesc);

                return true;
            }
            catch (Exception exception)
            {
                throw new Exception("Ebbs Computted setter in not set", exception);
            }

        }

        public bool ComputedSetter(Payment obj, Payment yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings)
        {
            return true;
        }

        public bool CheckBasicField(IExcelReader reader, ICounter counter)
        {

            ulong loanNumber;
            if (!ulong.TryParse(reader.GetValue(1), out loanNumber) || (loanNumber.ToString(CultureInfo.InvariantCulture).Length < 5))
            {
                _log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", reader.GetValue(1)));
                return false;
            }
            var narr = reader.GetValue(10).Trim() + reader.GetValue(11).Trim() +
                          reader.GetValue(12).Trim();
            var shouldBeExclude =
                _ePaymentExcludeCodes.Contains(string.Format("{0}@{1}", reader.GetValue(2).Trim(), narr));
            if (shouldBeExclude)
            {
                _log.Debug(string.Format("Payment of Account No {0} is Excluded Because TransCode : {1}, and TransDesc : {2}",
                                                reader.GetValue(1), reader.GetValue(2), narr));
                return false;
            }
            if (_eWriteoffAccounts.Contains(reader.GetValue(1)))
            {
                _log.Debug(string.Format("Payment of Account No {0} is Excluded Because It is Writeoff Account",
                                         reader.GetValue(1)));
                return false;
            }
            return true;
        }

        public bool IsRecordValid(Payment record)
        {
            return true;
        }
    }
}
