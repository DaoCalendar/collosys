using System.IO;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;

namespace ColloSys.FileUploadService.ClientDataServiceUOW
{
    public class RPaymentServiceUOW : FileUploadBulkDataUOW<RPayment>
    {
        #region ctor

        public RPaymentServiceUOW(FileScheduler file)
            : base(file)
        {
            var list = file.RPayments;
            InitObjectList(ref list);
        }

        #endregion

        #region perform updates

        protected override bool PerformUpdates(RPayment oldObj)
        {
            return false;
        }

        #endregion

        #region get unqiue

        protected override RPayment GetByUniqueKey(RPayment payment)
        {
            var session = SessionManager.GetCurrentSession();// SessionManager.GetSessionFactory().GetCurrentSession();
            return session.QueryOver<RPayment>()
                .Where(x => x.AccountNo == payment.AccountNo && x.FileDate == payment.FileDate)
                .SingleOrDefault<RPayment>();
        }

        #endregion

        #region public methods

        public new void Save(RPayment newVersion)
        {
            if (newVersion == null)
            {
                throw new InvalidDataException("Please make sure to pass not null list and file scheduler objects.");
            }

            base.Save(newVersion);
        }

        #endregion
    }
}