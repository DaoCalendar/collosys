using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;

namespace ColloSys.FileUploadService.ClientDataServiceUOW
{
    public class CLinerServiceUOW : FileUploadBulkDataUOW<CLiner>
    {
        #region ctor

        public CLinerServiceUOW(FileScheduler file)
            : base(file)
        {
            var listInfo = file.CLiners;
            InitObjectList(ref listInfo);
        }

        #endregion

        #region get unique

        protected override CLiner GetByUniqueKey(CLiner liner)
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<CLiner>()
                .Where(x => (x.FileDate == liner.FileDate.Date && x.AccountNo == liner.AccountNo))
                .SingleOrDefault<CLiner>();

            //return (from row in info.CLiners.ToList()
            //        where row.FileDate == liner.FileDate
            //        select row).SingleOrDefault();
        }

        protected override bool PerformUpdates(CLiner obj)
        {
            return true;
        }

        #endregion

        #region public methods

        //public new void Save( CLiner newCLiner)
        //{
        //    if ((newCLiner == null))
        //    {
        //        throw new InvalidDataException("Please make sure to pass not null list and file scheduler objects.");
        //    }


        //    base.Save(newCLiner);
        //}
        #endregion
    }
}