using System.IO;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;

namespace ColloSys.FileUploadService.ClientDataServiceUOW
{
    public class CacsActivityServiceUOW : FileUploadBulkDataUOW<CacsActivity>
    {
        #region ctor

        public CacsActivityServiceUOW(FileScheduler file)
            : base(file)
        {
            var list = file.CacsActivities;
            InitObjectList(ref list);
        }

        #endregion

        #region perform updates

        protected override bool PerformUpdates(CacsActivity obj)
        {
            return false;
        }

        #endregion

        #region get unique

        protected override CacsActivity GetByUniqueKey(CacsActivity newVersion)
        {
            var session = SessionManager.GetCurrentSession();// SessionManager.GetSessionFactory().GetCurrentSession();
            return session.QueryOver<CacsActivity>()
                          .Where(x => x.TelecallerId == newVersion.TelecallerId
                                      && x.AccountNumber == newVersion.AccountNumber
                                      && x.CallDatetime == newVersion.CallDatetime)
                          .SingleOrDefault<CacsActivity>();
        }

        #endregion

        #region public methods

        public new void Save(CacsActivity newVersion)
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