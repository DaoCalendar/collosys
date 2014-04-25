#region references

using System;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasRecordCreator;
using ReflectionExtension.ExcelReader;

#endregion


namespace ColloSys.FileUploader.AliasReader
{
    public class RlsPaymentWoPlpcRecordCreator : AliasPaymentRecordCreator
    {
        #region ctor
        private const uint AccountPosition = 1;
        private const uint AccountLength = 8;
        public RlsPaymentWoPlpcRecordCreator(FileScheduler fileShedular)
            : base(fileShedular,AccountPosition, AccountPosition)
        {
        }

        #endregion


        public override bool GetComputations(Payment record, IExcelReader reader)
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


    }
}
