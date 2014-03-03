using ColloSys.DataLayer.Domain;
using NHibernate;

namespace ColloSys.FileUploadService.ClientDataServiceUOW
{
    public class ELinerServiceUOW : FileUploadBulkDataUOW<ELiner>
    {
        #region ctor

        public ELinerServiceUOW(FileScheduler file)
            : base(file)
        {
            var listInfo = file.ELiners;
            InitObjectList(ref listInfo);
        }

        #endregion

        #region get unique

        

        protected override ELiner GetByUniqueKey(ELiner liner)
        {
            ISession session = _unit.CurrentSession;
            return session.QueryOver<ELiner>()
                .Where(x => x.FileDate == liner.FileDate && x.AccountNo == liner.AccountNo)
                .SingleOrDefault<ELiner>();
        }

        protected override bool PerformUpdates(ELiner obj)
        {
            return true;
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