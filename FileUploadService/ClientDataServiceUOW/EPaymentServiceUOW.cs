using System.IO;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;

namespace ColloSys.FileUploadService.ClientDataServiceUOW
{
    public class EPaymentServiceUOW : FileUploadBulkDataUOW<EPayment>
    {
        #region ctor

        public EPaymentServiceUOW(FileScheduler file)
            : base(file)
        {
            var list = file.EPayments;
            InitObjectList(ref list);
        }

        #endregion

        #region PerformUpdates

        protected override bool PerformUpdates(EPayment oldObj)
        {
            return false;
        }

        #endregion

        #region UniqueKey

        protected override EPayment GetByUniqueKey(EPayment payment)
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<EPayment>()
                .Where(x => x.AccountNo == payment.AccountNo && x.FileDate == payment.FileDate.Date)
                .SingleOrDefault<EPayment>();
        }

        #endregion        

        #region public methods

        public new void Save(EPayment newVersion)
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