#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.AliasPayment
{
    public class EbbsPaymentWoSmcRecordCreator : PaymentRecordCreator
    {
        #region ctor

        private const uint AccountPosition = 1;
        private const uint AccountLength = 11;
        private readonly IList<string> _ePaymentExcludeCodes;

        public EbbsPaymentWoSmcRecordCreator()
            : base(AccountPosition, AccountLength)
        {
            _ePaymentExcludeCodes = DbLayer.GetValuesFromKey(ColloSysEnums.Activities.FileUploader, "EPaymentExcludeCode");
        }

        #endregion

        #region overrides
        protected override bool GetComputations(Payment record, IExcelReader reader)
        {
            try
            {
                var shouldBeExclude = _ePaymentExcludeCodes.Contains(
                    string.Format("{0}@{1}", record.TransCode, record.TransDesc.Trim()));
                record.IsExcluded = shouldBeExclude;
                record.ExcludeReason = string.Format("Trans Code : {0}, Desc : {1}", record.TransCode, record.TransDesc);
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception("EbbsPaymentWoSmc ComputtedSetter failed!!", exception);
            }
        }

        protected override bool IsRecordValid(Payment record, ICounter counter)
        {
            if (record.TransDate.Month != FileScheduler.FileDate.Month)
            {
                counter.IncrementIgnoreRecord();
                return false;
            }
            return true;
        }
        #endregion
    }
}
