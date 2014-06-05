﻿#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using NHibernate.Linq;

#endregion

namespace AngularUI.Billing.newpolicy
{
    public class BillingPolicyApiController : BaseApiController<BillingPolicy>
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
                    .List<StkhHierarchy>();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
        }

        public class SubpolicyList
        {
            public List<BillingSubpolicy> IsInUseSubpolices = new List<BillingSubpolicy>();
            public List<BillingSubpolicy> NotInUseSubpolices = new List<BillingSubpolicy>();
        }

        [HttpGet]
        public HttpResponseMessage GetBillingSubpolicyList()
        {
            var billingSubpolicies = Session.Query<BillingSubpolicy>()
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