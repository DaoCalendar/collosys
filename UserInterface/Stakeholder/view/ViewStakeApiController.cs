#region references

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using AngularUI.Stakeholder.addedit.BasicInfo;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;

#endregion

namespace AngularUI.Stakeholder.view
{
    public class ViewStakeApiController : BaseApiController<Stakeholders>
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();

        [HttpGet]
        public HttpResponseMessage GetAllStakeHolders()
        {
            var list = StakeQuery.GetAllStakeholders();
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        [HttpPost]
        public HttpResponseMessage ApproveStakeholder(StkhId stakeholder)
        {
            var stkh = StakeQuery.Get(stakeholder.Id);
           
            if (stkh.ApprovalStatus == ColloSysEnums.ApproveStatus.Approved)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    ColloSysEnums.ApproveStatus.Approved.ToString());
            }
            stkh.ApprovalStatus = ColloSysEnums.ApproveStatus.Approved;
            StakeQuery.Save(stkh);

            if (!string.IsNullOrWhiteSpace(stkh.ExternalId))
            {
                var repo = new GUsersRepository();
                var user = AddEditStakeholder.CreateUser(stkh);
                repo.Save(user);
            }

            return Request.CreateResponse(HttpStatusCode.OK,
                ColloSysEnums.ApproveStatus.Approved.ToString());
        }

        [HttpPost]
        public HttpResponseMessage RejectStakeholder(StkhId stakeholder)
        {
            var stkh = StakeQuery.GetById(stakeholder.Id);
            if (stkh.ApprovalStatus == ColloSysEnums.ApproveStatus.Rejected)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    ColloSysEnums.ApproveStatus.Rejected.ToString());
            }
            stkh.ApprovalStatus = ColloSysEnums.ApproveStatus.Rejected;
            StakeQuery.Save(stkh);
            return Request.CreateResponse(HttpStatusCode.OK,
                ColloSysEnums.ApproveStatus.Rejected.ToString());
        }
    }

    public class StkhId
    {
        public Guid Id { get; set; }
    }
}