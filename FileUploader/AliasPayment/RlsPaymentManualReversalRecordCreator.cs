#region references

using ColloSys.DataLayer.ClientData;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.AliasPayment
{
    public class RlsPaymentManualReversalRecordCreator : PaymentRecordCreator
    {
        #region ctor
        private const uint AccountPosition = 3;
        private const uint AccountLength = 8;

        public RlsPaymentManualReversalRecordCreator() :
            base(AccountPosition, AccountLength) { }

        #endregion

        #region overrides
        protected override bool GetComputations(Payment record, IExcelReader reader)
        {
            record.IsDebit = (record.TransAmount > 0);
            return true;
        }
        #endregion
    }
}
