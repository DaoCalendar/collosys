#region references

using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using NLog;
using ReflectionExtension.ExcelReader;

#endregion


namespace ColloSys.FileUploader.AliasReader
{
   public class RlsPaymentManualReversalRecordCreator : IAliasRecordCreator<Payment>
    {
        #region ctor

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly FileScheduler _uploadedFile;

        public RlsPaymentManualReversalRecordCreator(FileScheduler fileShedular)
        {
            _uploadedFile = fileShedular;
        }

        #endregion

        public bool ComputedSetter(Payment record, IExcelReader reader, ICounter counter)
        {
            try
            {
                record.FileDate = _uploadedFile.FileDate;
                record.IsDebit = (record.TransAmount > 0);
                return true;
            }
            catch (Exception exception)
            {

                throw new Exception("Computted Record is Not Set", exception);
            }
        }

        public bool CheckBasicField(IExcelReader reader, ICounter counter)
        {
            string loanNo = reader.GetValue(3);
            ulong loanNumber;
            if (!ulong.TryParse(loanNo, out loanNumber) || (loanNumber.ToString(CultureInfo.InvariantCulture).Length < 3))
            {
                _log.Debug(string.Format("Account No {0} is not valid.", loanNo));
                return false;
            }
            return true;
        }

        public bool ComputedSetter(Payment obj, Payment yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings)
        {
            return true;
        }

        public bool IsRecordValid(Payment record)
        {
            return true;
        }
    }
}
