﻿#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.AliasPayment
{
    public class EbbsPaymentLinerRecordCreator : PaymentRecordCreator
    {
        #region ctor

        private readonly IList<string> _ePaymentExcludeCodes;
        private const uint AccountPosition = 1;
        private const uint AccountLength = 11;

        public EbbsPaymentLinerRecordCreator()
            : base(AccountPosition, AccountLength)
        {

            _ePaymentExcludeCodes = DbLayer.GetValuesFromKey(ColloSysEnums.Activities.FileUploader, "EPaymentExcludeCode");
        }

        #endregion

        #region overrides
        protected override bool GetComputations(Payment record)
        {
            try
            {
                record.IsDebit = (record.DebitAmount > 0);

                var shouldBeExclude = _ePaymentExcludeCodes.Contains(string.Format("{0}@{1}", record.TransCode, record.TransDesc.Trim()));
                record.IsExcluded = shouldBeExclude;

                record.ExcludeReason = string.Format("TransCode : {0}, and TransDesc : {1}", record.TransCode, record.TransDesc);

                return true;
            }
            catch (Exception exception)
            {
                throw new Exception("EbbsPaymentLinerRecordCreator Computted setter in not set", exception);
                //_log.Debug(string.Format("EbbsPaymentLinerRecordCreator Computted setter in not set {0}",exception));
               // return false;
            }
        }

        public override bool IsRecordValid(Payment record)
        {
            if (record.TransDate.Month != FileScheduler.FileDate.Month)
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }
            return record.TransDate.Month == FileScheduler.FileDate.Month;
        }
        #endregion
    }
}
