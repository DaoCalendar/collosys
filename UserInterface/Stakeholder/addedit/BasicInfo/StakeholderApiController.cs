#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using AngularUI.Stakeholder.addedit.Working;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;

#endregion

namespace AngularUI.Stakeholder.AddEdit2.BasicInfo
{
    //TODO: 1. remove pincode typeahead
    //TODO: 2. reporting list
    //TODO: 3. return HttpResponseMessage
    public class StakeholderApiController : BaseApiController<Stakeholders>
    {
        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();

        [HttpGet]
        public HttpResponseMessage GetAllHierarchies()
        {
            return Request.CreateResponse(HttpStatusCode.OK, HierarchyQuery.FilterBy(x => x.Hierarchy != "Developer"));
        }

        [HttpGet]
        public HttpResponseMessage CheckUserId(string id)
        {
            var idExists = StakeQuery.FilterBy(x => x.ExternalId == id);
            return Request.CreateResponse(HttpStatusCode.OK, idExists.Count > 0);
        }

        [HttpGet]
        public HttpResponseMessage GetReportsToData(Guid hierarchyId, ColloSysEnums.ReportingLevel level)
        {
            return Request.CreateResponse(HttpStatusCode.OK, WorkingPaymentHelper.GetReportsOnreportingLevel(hierarchyId, level));
        }

        [HttpPost]
        public HttpResponseMessage GetStakeForEdit(Guid stakeholderId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, StakeQuery.OnId(stakeholderId));
        }

        [HttpPost]
        public HttpResponseMessage SaveStake(Stakeholders data)
        {
            AddEditStakeholder.SetStakeholderObj(data);
            StakeQuery.Save(data);
            return Request.CreateResponse(HttpStatusCode.Created, data);
        }
    }
}


