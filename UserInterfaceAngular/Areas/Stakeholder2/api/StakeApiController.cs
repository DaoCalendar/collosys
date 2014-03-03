using System;
using System.Collections.Generic;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared.Attributes;

namespace ColloSys.UserInterface.Areas.Stakeholder2.api
{
    public class StakeApiController : ApiController
    {
        [HttpGet]
        [HttpTransaction]
        public IEnumerable<StkhHierarchy> GetAllHierarchies()
        {
            using (var session = SessionManager.GetCurrentSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.QueryOver<StkhHierarchy>().Where(x => x.Hierarchy != "Developer")
                          .List();
                    trans.Rollback();
                    return data;
                }
            }
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> GetReportingLevel()
        {
            return Enum.GetNames(typeof(ColloSysEnums.ReportingLevel));
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
                public void SaveHierarchy(StkhHierarchy stk)
        {
            using (var session = SessionManager.GetCurrentSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(stk);
                    transaction.Commit();
                }
                   
            }
        }

        //public StkhHierarchy SaveHierarchy(StkhHierarchy stk)
        //{
        //    var session = SessionManager.GetCurrentSession();
        //    session.SaveOrUpdate(stk);
        //    return stk;
        //}
    }
}
