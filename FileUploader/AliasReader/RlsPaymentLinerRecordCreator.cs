#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.ExcelReader;
using ColloSys.FileUploader.ExcelReaders.RecordSetter;
using NLog;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploader.AliasReader
{
    public class RlsPaymentLinerRecordCreator : IAliasRecordCreator<Payment>
    {
        #region ctor

        private readonly IList<string> _ePaymentExcludeCodes;
        private readonly List<string> _eWriteoffAccounts;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly FileScheduler _uploadedFile;
       
        public RlsPaymentLinerRecordCreator(IList<string> ePaymentExcludeCode, FileScheduler fileScheduler, List<string> eWriteoffAccounts)
        {
            _ePaymentExcludeCodes = ePaymentExcludeCode;
            _uploadedFile = fileScheduler;
            _eWriteoffAccounts = eWriteoffAccounts;
        }

        #endregion

        #region IAliasRecordCreator
        public bool ComputedSetter(Payment record, IExcelReader reader, ICounter counter)
        {
            record.FileDate = _uploadedFile.FileDate;
            record.IsDebit = (record.DebitAmount > 0);

            var shouldBeExclude = _ePaymentExcludeCodes.Contains(string.Format("{0}@{1}", record.TransCode, record.TransDesc.Trim()));
            record.IsExcluded = shouldBeExclude;
            record.ExcludeReason = string.Format("TransCode : {0}, and TransDesc : {1}", record.TransCode, record.TransDesc);
            return true;
        }

        public bool ComputedSetter(Payment obj, object yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckBasicField(IExcelReader reader, IEnumerable<FileMapping> mapings, ICounter counter)
        {
            var uniqColumnMapping = from d in mapings where (d.ActualColumn == "AccountNo") select d;
            var mapping = uniqColumnMapping.FirstOrDefault();
            if (mapping != null)
            {
                string data = reader.GetValue(mapping.Position);
                if (data != "" && SharedUtility.IsDigit(data))
                {
                    return true;
                }
            }
            counter.IncrementIgnoreRecord();
            counter.IncrementTotalRecords();

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
            if (_eWriteoffAccounts.Contains(reader.GetValue(2)))
            {
                _log.Debug(string.Format("Payment of Account No {0} is Excluded Because It is Writeoff Account",
                                         reader.GetValue(1)));
                return false;
            }

            return false;
        }

        public bool IsRecordValid(Payment record)
        {
            if (record.TransDate.Month == _uploadedFile.FileDate.Month) return true;
            _log.Debug(string.Format("Payment of Account No {0} is not valid, " +
                                     "Because Transaction month is not match with FileDate month",
                record.AccountNo));
            return false;
        }
        #endregion
    }
}
