using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;

namespace ColloSys.FileUploadService.ClientDataServiceUOW
{
    public class CWriteOffServiceUOW : FileUploadBulkDataUOW<CWriteoff>
    {
        #region ctor

        public CWriteOffServiceUOW(FileScheduler file)
            : base(file)
        {
            var listInfo = file.CWriteoffs;
            InitObjectList(ref listInfo);
        }

        #endregion

        

        #region get unique

        protected override CWriteoff GetByUniqueKey(CWriteoff liner)
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<CWriteoff>()
                .Where(x => (x.FileDate == liner.FileDate.Date && x.AccountNo == liner.AccountNo))
                .SingleOrDefault<CWriteoff>();
        }

        #endregion
    }
}