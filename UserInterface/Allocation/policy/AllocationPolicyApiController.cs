#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.AllocationBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using NHibernate.Linq;

#endregion

namespace AngularUI.Allocation.policy
{

    public class AllocationPolicyApiController : BaseApiController<AllocPolicy>
    {
        public struct StkhInfo
        {
            public Guid Id;
            public string Display;
        }

        [HttpPost]
        public HttpResponseMessage GetAllocSubpolicyList(PolicyDTO policy)
        {
            policy.SetPolicyId(Session);
            var allocSubpolicies = Session.Query<AllocSubpolicy>()
                .Where(x => x.Products == policy.Product)
                .Fetch(x => x.AllocRelations)
                .ToList();

            var manager = new SubpolicyManager();
            var list = manager.MoveSubpolicesToDTO(allocSubpolicies);
            manager.SeperateDTO2List(policy, list);
            return Request.CreateResponse(HttpStatusCode.OK, policy);
        }

        [HttpGet]
        public HttpResponseMessage Getcondition(Guid id)
        {
            var allocConditions = Session.Query<AllocCondition>()
                .Where(x => x.AllocSubpolicy.Id == id).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, allocConditions);
        }

        [HttpPost]
        public HttpResponseMessage SaveSubpolicy(SubpolicyDTO param)
        {
            var manager = new SubpolicySaver(GetUsername(), Session);
            var result = manager.ManageSubpolicyActivity(param);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}