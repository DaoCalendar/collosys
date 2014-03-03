using System.IO;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;

namespace ColloSys.FileUploadService.ClientDataServiceUOW
{
    public class CPaymentServiceUOW : FileUploadBulkDataUOW<CPayment>
    {
        #region ctor

        public CPaymentServiceUOW(FileScheduler file)
            : base(file)
        {
            var list = file.CPayments;
            InitObjectList(ref list);
        }

        #endregion

        #region perform updates

        protected override bool PerformUpdates(CPayment oldObj)
        {
            return false;
        }

        #endregion

        #region get unique

        protected override CPayment GetByUniqueKey(CPayment payment)
        {
            var session = SessionManager.GetCurrentSession();// SessionManager.GetSessionFactory().GetCurrentSession();
            return session.QueryOver<CPayment>()
                .Where(x => x.CardNo == payment.CardNo && x.FileDate == payment.FileDate.Date)
                .SingleOrDefault<CPayment>();
        }

        #endregion

        #region public methods

        public new void Save(CPayment newVersion)
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