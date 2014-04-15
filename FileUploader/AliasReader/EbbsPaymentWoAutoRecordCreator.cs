#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ReflectionExtension.ExcelReader;

#endregion


namespace ColloSys.FileUploader.AliasReader
{
    class EbbsPaymentWoAutoRecordCreator : AliasPaymentRecordCreator
    {
        #region ctor

       private readonly List<string> _ePaymentExcludeCodes;

        public EbbsPaymentWoAutoRecordCreator(FileScheduler fileShedular, List<string> ePaymentExcludeCodes)
            : base(fileShedular, 1, 11)
        {
            _ePaymentExcludeCodes = ePaymentExcludeCodes;
        }

        #endregion



        protected override bool GetComputations(Payment record, IExcelReader reader)
        {
            try
            {
                var shouldBeExclude = _ePaymentExcludeCodes.Contains(string.Format("{0}@{1}",
                    record.TransCode, record.TransDesc.Trim()));
                record.IsExcluded = shouldBeExclude;

                record.ExcludeReason = string.Format("TransCode : {0}, and TransDesc : {1}",
                    record.TransCode, record.TransDesc);

                return true;
            }
            catch (Exception exception)
            {
                throw new Exception("Ebbs Computted setter in not set", exception);
            }
        }
    }
}
