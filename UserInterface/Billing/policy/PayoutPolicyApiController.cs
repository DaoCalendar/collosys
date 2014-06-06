#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Billing.policy;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using NHibernate.Linq;

#endregion


namespace ColloSys.UserInterface.Areas.Billing.apiController
{
    
    public class PayoutPolicyApiController : BaseApiController<BillingPolicy>
    {
        [HttpGet]
        public HttpResponseMessage GetStakeholerOrHier(string policyfor)
        {
            if (policyfor == "Stakeholder")
            {
                var data = Session.QueryOver<Stakeholders>()
                    .Where(x => x.LeavingDate == null || x.LeavingDate >= DateTime.Today)
                    .List<Stakeholders>();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            else
            {
                var data = Session.QueryOver<StkhHierarchy>()
                    .Where(x => x.Hierarchy != "Developer")
                    .List<StkhHierarchy>().Distinct();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
        }

        public class SubpolicyList
        {
            public List<BillingSubpolicy> IsInUseSubpolices = new List<BillingSubpolicy>();
            public List<BillingSubpolicy> NotInUseSubpolices = new List<BillingSubpolicy>();
        }

        [HttpGet]
        public HttpResponseMessage GetBillingSubpolicyList(ScbEnums.Products product)
        {
            var billingSubpolicies = Session.Query<BillingSubpolicy>()
                .Where(x => x.Products == product)
                .Fetch(x => x.BillingRelations)
                .ToList();


            var list = new SubpolicyList();

            foreach (var subpolicy in billingSubpolicies)
            {
                if (subpolicy.BillingRelations.Count == 0 || subpolicy.BillingRelations == null)
                {
                    list.NotInUseSubpolices.Add(subpolicy);
                    continue;
                }

                if (subpolicy.BillingRelations.Count(x => x.EndDate == null) > 0)
                {
                    list.IsInUseSubpolices.Add(subpolicy);
                    continue;
                }

                var relations = subpolicy.BillingRelations.OrderByDescending(x => x.StartDate).First();
                if (relations.EndDate < DateTime.Today)
                {
                    list.NotInUseSubpolices.Add(subpolicy);
                    continue;
                }

                // end is future or null (active)
                list.IsInUseSubpolices.Add(subpolicy);
            }

            foreach (var subpolicy in billingSubpolicies)
            {
                if (subpolicy.BillingRelations.Count <= 1 || subpolicy.BillingRelations == null)
                {
                    continue;
                }
                var firstRelation = subpolicy.BillingRelations.OrderByDescending(x => x.StartDate).First();
                subpolicy.BillingRelations = new List<BillingRelation> { firstRelation };
            }

            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        [HttpGet]
        public HttpResponseMessage GetBillingTokens(Guid id)
        {
            var billingTokens = Session.Query<BillTokens>()
                .Where(x => x.BillingSubpolicy.Id == id).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, billingTokens);
        }

        public HttpResponseMessage SaveSubpolicy(SubpolicyRelationManager subpolicy)
        {
            subpolicy.Username = GetUsername();
            subpolicy.Session = Session;

            switch (subpolicy.Activity)
            {
                case "activate":
                    subpolicy.ActivateSubpolicy();
                    break;
                case "deactivate":
                    subpolicy.DeactivateSubpolicy();
                    break;
                case "approve":
                    subpolicy.ApproveSubpolicy();
                    break;
                case "reject":
                    subpolicy.RejectSubpolicy();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("subpolicy");
            }
            return Request.CreateResponse(HttpStatusCode.OK, "success");
        }
    }
}
