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
   public class EbbsPaymentWoSmcRecordCreator : AliasPaymentRecordCreator
    {
        #region ctor

        private const uint AccountPosition = 1;
        private const uint AccountLength = 11;
        private readonly List<string> _ePaymentExcludeCodes;

        public EbbsPaymentWoSmcRecordCreator(FileScheduler fileShedular, List<string> ePaymentExcludeCodes)
            : base(fileShedular, AccountPosition, AccountLength)
        {
            _ePaymentExcludeCodes = ePaymentExcludeCodes;
        }

        #endregion


        public override bool GetComputations(Payment record, IExcelReader reader)
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

    }
}
