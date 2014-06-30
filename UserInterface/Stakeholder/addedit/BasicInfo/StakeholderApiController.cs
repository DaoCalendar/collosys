#region references

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Generic.home;
using AngularUI.Shared.apis;
using AngularUI.Stakeholder.addedit.Working;
using AngularUI.Stakeholder.view;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using Glimpse.Core.ClientScript;

#endregion

namespace AngularUI.Stakeholder.addedit.BasicInfo
{
    public class StakeholderApiController : BaseApiController<Stakeholders>
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();

        #region get

        [HttpGet]
        public HttpResponseMessage GetAllHierarchies()
        {
            var hierarchyQuery = new HierarchyQueryBuilder();
            var hierarchies = hierarchyQuery.GetAllHierarchies();
            return Request.CreateResponse(HttpStatusCode.OK, hierarchies);
        }

        [HttpGet]
        public HttpResponseMessage CheckUserId(string id)
        {
            var idExists = StakeQuery.FilterBy(x => x.ExternalId == id);
            return Request.CreateResponse(HttpStatusCode.OK, idExists.Count > 0);
        }

        [HttpGet]
        public HttpResponseMessage GetReportsToList(Guid hierarchyId, ColloSysEnums.ReportingLevel level)
        {
            var list = WorkingPaymentHelper.GetStkhByReportingLevel(hierarchyId, level);
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        [HttpGet]
        public HttpResponseMessage GetStakeholder(Guid stakeholderId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, StakeQuery.OnId(stakeholderId));
        }

        #endregion

        #region post

        [HttpPost]
        public HttpResponseMessage SaveStakeholder(Stakeholders data)
        {
            AddEditStakeholder.SetStakeholderObj(data);
            StakeQuery.Save(data);

            if (data.ApprovalStatus == ColloSysEnums.ApproveStatus.Submitted)
            {
                var notify = new StakeholderNotificationManager(GetUsername());
                notify.NotifyNewStakeholder(data);
            }

            return Request.CreateResponse(HttpStatusCode.Created, data);
        }

        [HttpPost]
        public HttpResponseMessage ApproveStakeholder(StkhId stakeholder)
        {
            var stkh = StakeQuery.Get(stakeholder.Id);
            if (stkh.ApprovalStatus == ColloSysEnums.ApproveStatus.Approved ||
                stkh.ApprovalStatus == ColloSysEnums.ApproveStatus.Changed)
            {
                stkh.ApprovalStatus = ColloSysEnums.ApproveStatus.Changed;
                StakeQuery.Save(stkh);
                return Request.CreateResponse(HttpStatusCode.OK, stkh);
            }

            stkh.ApprovalStatus = ColloSysEnums.ApproveStatus.Approved;
            StakeQuery.Save(stkh);

            if (!string.IsNullOrWhiteSpace(stkh.ExternalId))
            {
                var repo = new GUsersRepository();
                var user = AddEditStakeholder.CreateUser(stkh);
                repo.Save(user);
            }

            return Request.CreateResponse(HttpStatusCode.OK, stkh);
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
                stkh);
        }

        #endregion
    }
}


