#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;
using NHibernate.Linq;

#endregion

namespace AngularUI.Billing.policy
{
    public class PayoutPolicyApiController : BaseApiController<BillingPolicy>
    {
        public struct StkhInfo
        {
            public Guid Id;
            public string Display;
        }
        [HttpPost]
        public HttpResponseMessage GetStakeholerOrHier(PolicyDTO policy)
        {
            if (policy.PolicyFor == ColloSysEnums.PolicyOn.Stakeholder)
            {
                var data = Session.Query<Stakeholders>()
                    //.Where(x => x.LeavingDate == null || x.LeavingDate >= DateTime.Today)
                    .ToList();
                //private StakeQueryBuilder stkhrepo = new StakeQueryBuilder();
                //var data = stkhrepo.OnProduct(policy.Product);

                var stkhData = new List<StkhInfo>();
                foreach (var stkh in data)
                {
                    var info = new StkhInfo
                    {
                        Id = stkh.Id,
                        Display =
                            string.IsNullOrWhiteSpace(stkh.ExternalId)
                                ? stkh.Name
                                : string.Format("{0} - {1}", stkh.Name, stkh.ExternalId)
                    };
                    stkhData.Add(info);
                }

                return Request.CreateResponse(HttpStatusCode.OK, stkhData);
            }
            else
            {
                var data = Session.Query<StkhHierarchy>()
                    .Where(x => x.Hierarchy != "Developer")
                    .ToList();
                var hierData = new List<StkhInfo>();
                foreach (var hier in data)
                {
                    var info = new StkhInfo
                    {
                        Id = hier.Id,
                        Display = string.Format("{0} - {1}", hier.Designation, hier.Hierarchy)
                    };
                    hierData.Add(info);
                }

                return Request.CreateResponse(HttpStatusCode.OK, hierData);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetBillingSubpolicyList(PolicyDTO policy)
        {
            policy.SetPolicyId(Session);
            var billingSubpolicies = Session.Query<BillingSubpolicy>()
                .Where(x => x.Products == policy.Product && x.PolicyType == policy.PolicyType)
                .Fetch(x => x.BillingRelations)
                .ToList();

            var manager = new SubpolicyManager();
            var list = manager.MoveSubpolicesToDTO(billingSubpolicies);
            manager.SeperateDTO2List(policy, list);
            return Request.CreateResponse(HttpStatusCode.OK, policy);
        }

        [HttpGet]
        public HttpResponseMessage GetBillingTokens(Guid id)
        {
            var billingTokens = Session.Query<BillTokens>()
                .Where(x => x.BillingSubpolicy.Id == id).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, billingTokens);
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
