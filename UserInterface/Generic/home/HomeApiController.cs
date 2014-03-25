using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace AngularUI.Generic.home
{
    public class HomeApiController : ApiController
    {
        [HttpGet]
        [HttpSession]
        public HttpResponseMessage GetData(string currentUser)
        {
            var id = currentUser;
            var session = SessionManager.GetCurrentSession();
            var data = new
            {
                stakeholder = session.QueryOver<Stakeholders>().Where(x => x.ApprovedBy == id
                    && x.Status == ColloSysEnums.ApproveStatus.Submitted).List().Count(),
                working = session.QueryOver<StkhWorking>().Where(x => x.ApprovedBy == id
                    && x.Status == ColloSysEnums.ApproveStatus.Submitted).List().Count(),
                payment = session.QueryOver<StkhPayment>().Where(x => x.ApprovedBy == id
                    && x.Status == ColloSysEnums.ApproveStatus.Submitted).List().Count(),
                allocation = session.QueryOver<AllocRelation>().Where(x => x.ApprovedBy == id
                    && x.Status == ColloSysEnums.ApproveStatus.Submitted).List().Count(),
                allocationpolicy = session.QueryOver<AllocRelation>().Where(x => x.ApprovedBy == id
                    && x.Status == ColloSysEnums.ApproveStatus.Submitted).List().Count()
            };
            return Request.CreateResponse(HttpStatusCode.Created, data);
        }
    }
}
