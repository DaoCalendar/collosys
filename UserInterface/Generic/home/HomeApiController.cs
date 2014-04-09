using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NHibernate.Criterion;


namespace AngularUI.Generic.home
{
    public class HomeApiController : ApiController
    {
        [HttpGet]

        public HttpResponseMessage GetData(string currentUser)
        {
            var id = currentUser;
            var session = SessionManager.GetCurrentSession();
            var data = new
            {
                stakeholder = session.QueryOver<Stakeholders>()
                    .Where(x => x.ApprovedBy == id && x.Status == ColloSysEnums.ApproveStatus.Submitted)
                    .Select(Projections.RowCount()).FutureValue<int>().Value,
                working = session.QueryOver<StkhWorking>()
                    .Where(x => x.ApprovedBy == id && x.Status == ColloSysEnums.ApproveStatus.Submitted)
                    .Select(Projections.RowCount()).FutureValue<int>().Value,
                payment = session.QueryOver<StkhPayment>()
                    .Where(x => x.ApprovedBy == id && x.Status == ColloSysEnums.ApproveStatus.Submitted)
                    .Select(Projections.RowCount()).FutureValue<int>().Value,
            };
            return Request.CreateResponse(HttpStatusCode.Created, data);
        }
    }
}
