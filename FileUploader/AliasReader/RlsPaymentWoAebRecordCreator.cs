#region references
using System;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ReflectionExtension.ExcelReader;

#endregion


namespace ColloSys.FileUploader.AliasReader
{
    public class RlsPaymentWoAebRecordCreator : AliasPaymentRecordCreator
    {
        #region ctor

        public RlsPaymentWoAebRecordCreator(FileScheduler fileShedular):base(fileShedular,1,8)
        {
           
        }

        #endregion

        protected override bool GetComputations(Payment record, IExcelReader reader)
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
