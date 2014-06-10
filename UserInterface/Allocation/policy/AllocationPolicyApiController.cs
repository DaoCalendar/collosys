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
    //new code
    //public class PayoutPolicyApiController : BaseApiController<AllocPolicy>
    //{
    //    public struct StkhInfo
    //    {
    //        public Guid Id;
    //        public string Display;
    //    }
    //  //  [HttpPost]
    //    //public HttpResponseMessage GetStakeholerOrHier(PolicyDTO policy)
    //    //{
    //    //    if (policy.PolicyFor == ColloSysEnums.PolicyOn.Stakeholder)
    //    //    {
    //    //        var data = Session.Query<Stakeholders>()
    //    //            //.Where(x => x.LeavingDate == null || x.LeavingDate >= DateTime.Today)
    //    //            .ToList();
    //    //        //private StakeQueryBuilder stkhrepo = new StakeQueryBuilder();
    //    //        //var data = stkhrepo.OnProduct(policy.Product);

    //    //        var stkhData = new List<StkhInfo>();
    //    //        foreach (var stkh in data)
    //    //        {
    //    //            var info = new StkhInfo
    //    //            {
    //    //                Id = stkh.Id,
    //    //                Display =
    //    //                    string.IsNullOrWhiteSpace(stkh.ExternalId)
    //    //                        ? stkh.Name
    //    //                        : string.Format("{0} - {1}", stkh.Name, stkh.ExternalId)
    //    //            };
    //    //            stkhData.Add(info);
    //    //        }

    //    //        return Request.CreateResponse(HttpStatusCode.OK, stkhData);
    //    //    }
    //    //    else
    //    //    {
    //    //        var data = Session.Query<StkhHierarchy>()
    //    //            .Where(x => x.Hierarchy != "Developer")
    //    //            .ToList();
    //    //        var hierData = new List<StkhInfo>();
    //    //        foreach (var hier in data)
    //    //        {
    //    //            var info = new StkhInfo
    //    //            {
    //    //                Id = hier.Id,
    //    //                Display = string.Format("{0} - {1}", hier.Designation, hier.Hierarchy)
    //    //            };
    //    //            hierData.Add(info);
    //    //        }

    //    //        return Request.CreateResponse(HttpStatusCode.OK, hierData);
    //    //    }
    //    //}

    //    [HttpPost]
    //    public HttpResponseMessage GetBillingSubpolicyList(PolicyDTO policy)
    //    {
    //        policy.SetPolicyId(Session);
    //        var billingSubpolicies = Session.Query<AllocSubpolicy>()
    //            .Where(x => x.Products == policy.Product)
    //            .Fetch(x => x.AllocRelations)
    //            .ToList();

    //        var manager = new SubpolicyManager();
    //        var list = manager.MoveSubpolicesToDTO(billingSubpolicies);
    //        manager.SeperateDTO2List(policy, list);
    //        return Request.CreateResponse(HttpStatusCode.OK, policy);
    //    }

    //    [HttpGet]
    //    public HttpResponseMessage GetBillingTokens(Guid id)
    //    {
    //        var billingTokens = Session.Query<AllocTokens>()
    //            .Where(x => x.AllocSubpolicy.Id == id).ToList();
    //        return Request.CreateResponse(HttpStatusCode.OK, billingTokens);
    //    }

    //    [HttpPost]
    //    public HttpResponseMessage SaveSubpolicy(SubpolicyDTO param)
    //    {
    //        var manager = new SubpolicySaver(GetUsername(), Session);
    //        var result = manager.ManageSubpolicyActivity(param);
    //        return Request.CreateResponse(HttpStatusCode.OK, result);
    //    }
    //}

    //oldcode


    public class AlloPolicyVm
    {
        public AllocPolicy AllocPolicy { get; set; }

        public IList<AllocSubpolicy> UnUsedSubpolicies { get; set; }
    }

    public class AllocationPolicyApiController : BaseApiController<AllocPolicy>
    {
        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        private static readonly AllocPolicyBuilder AllocPolicyBuilder = new AllocPolicyBuilder();
        private static readonly AllocSubpolicyBuilder AllocSubpolicyBuilder = new AllocSubpolicyBuilder();
        private static readonly AllocRelationBuilder AllocRelationBuilder = new AllocRelationBuilder();
        #region Get

        [HttpGet]

        public HttpResponseMessage GetProducts()
        {
            var data = ProductConfigBuilder.GetProducts();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]

        public HttpResponseMessage GetAllocPolicy(ScbEnums.Products products, ScbEnums.Category category)
        {

            var allocPolicy = AllocPolicyBuilder.NonApproved(products, category);
            allocPolicy.AllocRelations = allocPolicy.AllocRelations.Distinct().ToList();

            // create new alloc policy
            var savedAllocSubpolicyIds = new List<Guid>();
            // make alloc subpolicy empty, json serialization hack
            foreach (var relations in allocPolicy.AllocRelations)
            {
                relations.AllocSubpolicy.MakeEmpty();

                if (relations.AllocSubpolicy.Stakeholder != null)
                    relations.AllocSubpolicy.Stakeholder.MakeEmpty();

                savedAllocSubpolicyIds.Add(relations.AllocSubpolicy.Id);
            }

            var allocSubpolicies = AllocSubpolicyBuilder.OnProductCategorySubPolicyGuids(products, category,
                                                                                         savedAllocSubpolicyIds).ToList();

            var allocPolicyVm = new AlloPolicyVm() { AllocPolicy = allocPolicy, UnUsedSubpolicies = allocSubpolicies };

            return Request.CreateResponse(HttpStatusCode.OK, allocPolicyVm);
        }

        #endregion

        #region override methods

        protected override AllocPolicy BasePost(AllocPolicy obj)
        {
            // murge BillingRelation into added BillingPolicy
            foreach (var billingRelation in obj.AllocRelations)
            {
                billingRelation.AllocSubpolicy = AllocSubpolicyBuilder.Get(billingRelation.AllocSubpolicy.Id);
                billingRelation.AllocPolicy = obj;
            }

            AllocPolicyBuilder.Save(obj);
            return obj;
        }

        protected override AllocPolicy BasePut(Guid id, AllocPolicy obj)
        {
            var deletedRelation = new List<AllocRelation>();

            foreach (var billingRelation in obj.AllocRelations)
            {
                billingRelation.AllocPolicy = obj;

                if (billingRelation.Status != ColloSysEnums.ApproveStatus.Approved ||
                    !string.IsNullOrWhiteSpace(billingRelation.ApprovedBy))
                {
                    billingRelation.ApprovedBy = GetReportsToId();
                    continue;
                }

                // approve subpolicy
                billingRelation.ApprovedBy = HttpContext.Current.User.Identity.Name;
                billingRelation.ApprovedOn = DateTime.Now;

                // delete old subpolicy relation
                if (billingRelation.OrigEntityId == Guid.Empty)
                    continue;

                var delrelation = AllocRelationBuilder.Load(billingRelation.OrigEntityId);
                AllocRelationBuilder.Delete(delrelation);
                billingRelation.OrigEntityId = Guid.Empty;
            }

            foreach (var billingRelation in obj.AllocRelations)
            {
                if (deletedRelation.Any(x => x.Id == billingRelation.Id))
                {
                    AllocRelationBuilder.Delete(billingRelation);
                    continue;
                }
                AllocRelationBuilder.Save(billingRelation);
            }

            AllocPolicyBuilder.Save(obj);
            //var query = AllocPolicyBuilder.ApplyRelations();
            //query.Where(x => x.Id == obj.Id);
            //var data = AllocPolicyBuilder.Execute(query).FirstOrDefault();
            return obj;
        }

        public string GetReportsToId()
        {
            var reportsToId = string.Empty;
            var currUserId = GetUsername();
            var currUser = StakeQuery.FilterBy(x => x.ExternalId == currUserId).SingleOrDefault();

            if (currUser != null && currUser.ReportingManager != Guid.Empty)
            {
                reportsToId = StakeQuery.OnIdWithAllReferences(currUser.ReportingManager).ExternalId;
            }
            return reportsToId;
        }

        #endregion

        #region Delete
        [HttpDelete]
        public virtual HttpResponseMessage RejectSubpolicy(Guid id)
        {
            try
            {
                if (id.Equals(Guid.Empty))
                {
                    throw new InvalidDataException("Id provided is empty. No entity with such id exist.");
                }

                var relation = AllocRelationBuilder.Load(id);
                AllocRelationBuilder.Delete(relation);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            }
        }
        #endregion

        [HttpGet]
        public HttpResponseMessage ApproveRelation(Guid relationId)
        {
            var relation = AllocRelationBuilder.Get(relationId);
            relation.Status = ColloSysEnums.ApproveStatus.Approved;
            if (relation.OrigEntityId != Guid.Empty)
            {
                var origRelation = AllocRelationBuilder.Get(relation.OrigEntityId);
                AllocRelationBuilder.Delete(origRelation);
            }
            relation.OrigEntityId = Guid.Empty;
            AllocRelationBuilder.Save(relation);
            return Request.CreateResponse(HttpStatusCode.OK, "");
        }


    }
}