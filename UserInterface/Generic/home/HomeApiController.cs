using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using NHibernate.Criterion;


namespace AngularUI.Generic.home
{
    public class HomeApiController : ApiController
    {


        [HttpGet]

        public HttpResponseMessage GetData(string currentUser)
        {
            List<Guid> list = new List<Guid>();

            var session = SessionManager.GetCurrentSession();

            var stakeholders = session.QueryOver<Stakeholders>()
                                      .Where(
                                          x =>
                                          x.ApprovedBy == currentUser &&
                                          x.Status == ColloSysEnums.ApproveStatus.Submitted)
                                          .Select(x => x.Id)
                                          .List<Guid>();
            list.AddRange(stakeholders);
            var working = session.QueryOver<StkhWorking>()
                                 .Where(
                                     x =>
                                     x.ApprovedBy == currentUser &&
                                     x.Status == ColloSysEnums.ApproveStatus.Submitted)
                                .Select(x => x.Stakeholder.Id)
                                .List<Guid>();
            list.AddRange(working);
            var payment = session.QueryOver<StkhPayment>()
                                 .Where(
                                     x =>
                                     x.ApprovedBy == currentUser && x.Status == ColloSysEnums.ApproveStatus.Submitted)
                                 .Select(x => x.Stakeholder.Id)
                                .List<Guid>();
            list.AddRange(payment);
          
            var data = new
            {
                stakeholders = list.Distinct().Count(),
                allocation = session.QueryOver<AllocRelation>()
                    .Where(x => x.ApprovedBy == currentUser && x.Status == ColloSysEnums.ApproveStatus.Submitted)
                    .Select(Projections.RowCount()).FutureValue<int>().Value,
                billing = session.QueryOver<BillingRelation>()
                     .Where(x => x.ApprovedBy == currentUser && x.Status == ColloSysEnums.ApproveStatus.Submitted)
                     .Select(Projections.RowCount()).FutureValue<int>().Value,
            };
            return Request.CreateResponse(HttpStatusCode.Created, data);
        }
    }
}
