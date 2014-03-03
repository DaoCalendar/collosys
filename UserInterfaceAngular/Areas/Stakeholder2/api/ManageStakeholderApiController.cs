using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Linq;

namespace ColloSys.UserInterface.Areas.Stakeholder2.api
{
    public class ManageStakeholderApiController : ApiController
    {

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetStakeholders(string name)
        {
            var session = SessionManager.GetCurrentSession();
            var data = session.Query<Stakeholders>()
                                      .Fetch(x => x.Hierarchy)
                                      .Fetch(x => x.StkhWorkings)
                                      .Fetch(x=>x.StkhPayments)
                                      .Fetch(x=>x.StkhRegistrations)
                                      .Where(x => x.Name.StartsWith(name))
                                      .Take(10)
                                      .ToList();
            return data;
            //using (var session = SessionManager.GetCurrentSession())
            //{
            //    using (var trans = session.BeginTransaction())
            //    {
            //        var data = session.Query<Stakeholders>()
            //                          .Fetch(x => x.Hierarchy)
            //                          .Fetch(x=>x.StkhWorkings)
            //                          .Where(x => x.Name.StartsWith(name))
            //                          .Take(10)
            //                          .ToList();
            //        trans.Rollback();
            //        return data;
            //    }
            //}
        }
    }
}
