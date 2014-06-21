#region references

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using AngularUI.Stakeholder.addedit.Working;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;

#endregion

namespace AngularUI.Stakeholder.addedit.BasicInfo
{
    public class StakeholderApiController : BaseApiController<Stakeholders>
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();

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

        [HttpPost]
        public HttpResponseMessage SaveStakeholder(Stakeholders data)
        {
            AddEditStakeholder.SetStakeholderObj(data);
            StakeQuery.Save(data);
            return Request.CreateResponse(HttpStatusCode.Created, data);
        }
    }
}


