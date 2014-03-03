using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using System.IO;

namespace ColloSys.FileUploadService.ClientDataServiceUOW
{
    public class CUnbilledServiceUOW : FileUploadBulkDataUOW<CUnbilled>
    {
        #region ctor

        public CUnbilledServiceUOW(FileScheduler file)
            : base(file)
        {
            var list = file.CUnbilleds;
            InitObjectList(ref list);
        }

        #endregion

        #region perform updates

        protected override bool PerformUpdates(CUnbilled oldObj)
        {
            return false;
        }

        #endregion

        #region get unique

        protected override CUnbilled GetByUniqueKey(CUnbilled unbilled)
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<CUnbilled>()
                .Where(x => x.CardNo == unbilled.CardNo && x.FileDate == unbilled.FileDate.Date)
                .SingleOrDefault<CUnbilled>();
        }

        #endregion

        #region public methods
        
        public new void Save(CUnbilled newVersion)
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
