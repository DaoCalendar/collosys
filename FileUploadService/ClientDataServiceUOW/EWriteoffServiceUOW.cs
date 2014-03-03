using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;

namespace ColloSys.FileUploadService.ClientDataServiceUOW
{
    public class EWriteoffServiceUOW : FileUploadBulkDataUOW<EWriteoff>
    {
        #region ctor

        public EWriteoffServiceUOW(FileScheduler file)
            : base(file)
        {
            var listInfo = file.EWriteoffs;
            InitObjectList(ref listInfo);
        }

        #endregion

        #region perform updates


        #endregion

        #region get unique

        protected override EWriteoff GetByUniqueKey(EWriteoff liner)
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<EWriteoff>()
                .Where(x => ((x.FileDate == liner.FileDate.Date) && (x.AccountNo == liner.AccountNo)))
                .SingleOrDefault<EWriteoff>();
        }

        #endregion
    }
}