#region references

using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.AliasPayment
{
    public class RlsPaymentLinerRecordCreator : PaymentRecordCreator
    {
        #region ctor

        private const uint AccountPosition = 3;
        private const uint AccountLength = 8;

        private readonly IList<int> _paymentcodes;

        public RlsPaymentLinerRecordCreator()
            : base(AccountPosition, AccountLength)
        {
            _paymentcodes = new List<int> { 179, 201, 203, 891 };
        }

        #endregion

        #region overrides
        protected override bool GetComputations(Payment record, IExcelReader reader)
        {
            record.IsDebit = _paymentcodes.Contains(record.TransCode);

            var amountdiff = record.DebitAmount - (record.CreditAmount.HasValue ? record.CreditAmount.Value : 0);
            record.TransAmount = amountdiff * (amountdiff < 0 ? -1 : 1);
            return true;
        }
        #endregion
    }
}
