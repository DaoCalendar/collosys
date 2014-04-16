#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploader.AliasReader
{
   public class EbbsPaymentLinerRecordCreator : AliasPaymentRecordCreator
    {
        #region ctor

        private readonly List<string> _ePaymentExcludeCodes;
        private const uint AccountPosition = 1;
        private const uint AccountLength = 11;
       private readonly FileScheduler _scheduler;

        public EbbsPaymentLinerRecordCreator(FileScheduler fileScheduler, List<string> ePaymentExcludeCodes)
            : base(fileScheduler, AccountPosition, AccountLength)
        {
            _ePaymentExcludeCodes = ePaymentExcludeCodes;
            _scheduler = fileScheduler;
        }

        #endregion

        public override bool GetComputations(Payment record, IExcelReader reader)
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
            }
        }

       public override bool IsRecordValid(Payment record,ICounter counter)
       {
           if (record.TransDate.Month != _scheduler.FileDate.Month)
           {
               counter.IncrementIgnoreRecord();
               return false;
           }
           return record.TransDate.Month == _scheduler.FileDate.Month;
       }
    }
}
