using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Services.Shared;
using ColloSys.UserInterface.Shared.Attributes;

namespace ColloSys.UserInterface.Shared
{
    public class HomeApiController : ApiController
    {
        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetData()
        {
            var id = System.Web.HttpContext.Current.User.Identity.Name;
            var session = SessionManager.GetCurrentSession();
            //var permission = AuthService.GetPremissionsForCurrentUser();
            var data = new PendingApprovalData
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
