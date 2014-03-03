using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;

namespace ColloSys.FileUploadService.ClientDataServiceUOW
{
    public class RWriteOffServiceUOW : FileUploadBulkDataUOW<RWriteoff>
    {
        #region cnst

        public RWriteOffServiceUOW(FileScheduler file)
            : base(file)
        {
            var listInfo = file.RWriteoffs;
            InitObjectList(ref listInfo);
        }

        #endregion

        #region UniqueKey

        protected override RWriteoff GetByUniqueKey(RWriteoff liner)
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<RWriteoff>()
                .Where(x => ((x.FileDate == liner.FileDate.Date) && (x.AccountNo == liner.AccountNo)))
                .SingleOrDefault<RWriteoff>();
        }

        #endregion

    }
}




//ISession session = SessionManager.GetSessionFactory().GetCurrentSession();
//return session.QueryOver<HelperClass>()
//    .Where(x => ((x.FileDate == liner.FileDate) && (x.InfoClass.Id == info.Id)))
//    .SingleOrDefault<HelperClass>();

//return (from row in info.ELiners.ToList()
//        where row.FileDate == liner.FileDate
//        select row).SingleOrDefault();


//var test = session.QueryOver<EInfo>().Where(x => x.LoanNo == info.LoanNo).List();
//return session.CreateCriteria<EInfo>()
//              .Add(Restrictions.Eq("LoanNo", (UInt64)12345679063))
//              .List<EInfo>().FirstOrDefault();