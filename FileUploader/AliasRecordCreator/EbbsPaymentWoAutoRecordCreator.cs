﻿#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasRecordCreator;
using ReflectionExtension.ExcelReader;

#endregion


namespace ColloSys.FileUploader.AliasReader
{
    public class EbbsPaymentWoAutoRecordCreator : AliasPaymentRecordCreator
    {
        #region ctor

        private const uint AccountPosition = 1;
        private const uint AccountLength = 11;
       private readonly List<string> _ePaymentExcludeCodes;
        private readonly FileScheduler _scheduler;

        public EbbsPaymentWoAutoRecordCreator(FileScheduler fileShedular, List<string> ePaymentExcludeCodes)
            : base(fileShedular, AccountPosition, AccountLength)
        {
            _ePaymentExcludeCodes = ePaymentExcludeCodes;
            _scheduler = fileShedular;
        }

        #endregion



        public override bool GetComputations(Payment record, IExcelReader reader)
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
        public override bool IsRecordValid(Payment record,ICounter counter)
        {
            if (record.TransDate.Month != _scheduler.FileDate.Month)
            {
                counter.IncrementIgnoreRecord();
                return false;
            }
            return true;
        }
    }
}
