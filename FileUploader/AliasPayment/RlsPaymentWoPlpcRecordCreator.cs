#region references

using System;
using ColloSys.DataLayer.ClientData;
using ReflectionExtension.ExcelReader;

#endregion


namespace ColloSys.FileUploaderService.AliasPayment
{
    public class RlsPaymentWoPlpcRecordCreator : PaymentRecordCreator
    {
        #region ctor
        private const uint AccountPosition = 1;
        private const uint AccountLength = 8;

        public RlsPaymentWoPlpcRecordCreator()
            : base(AccountPosition, AccountLength)
        {
        }
        #endregion

        #region overrides
        protected override bool GetComputations(Payment record)
        {
            try
            {
                record.IsDebit = (record.TransAmount > 0);
                return true;
            }
            catch (Exception exception)
            {

                throw new Exception("Computted Record is Not Set", exception);
            }
        }
        #endregion
    }
}
