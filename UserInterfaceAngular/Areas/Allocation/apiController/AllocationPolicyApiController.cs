﻿#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.AllocationBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.SqlCommand;
using NHibernate.Transform;

#endregion

namespace UserInterfaceAngular.Areas.Allocation.apiController
{
    public class AlloPolicyVm
    {
        public AllocPolicy AllocPolicy { get; set; }

        public IList<AllocSubpolicy> UnUsedSubpolicies { get; set; }
    }

    public class AllocationPolicyApiController : BaseApiController<AllocPolicy>
    {
        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder(); 
        private static readonly StakeQueryBuilder StakeQuery=new StakeQueryBuilder();
        private static readonly AllocPolicyBuilder AllocPolicyBuilder=new AllocPolicyBuilder();
        private static readonly AllocSubpolicyBuilder AllocSubpolicyBuilder=new AllocSubpolicyBuilder();
        private static readonly AllocRelationBuilder AllocRelationBuilder=new AllocRelationBuilder();
        #region Get

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetProducts()
        {
            var data = ProductConfigBuilder.GetProducts();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetAllocPolicy(ScbEnums.Products products, ScbEnums.Category category)
        {

            var allocPolicy = AllocPolicyBuilder.NonApproved(products, category);

            // create new alloc policy
            var savedAllocSubpolicyIds = new List<Guid>();
            if (allocPolicy == null)
            {
                allocPolicy = new AllocPolicy() { Name = products + "_" + category, Products = products, Category = category };
            }
            else
            {
                // make alloc subpolicy empty, json serialization hack
                foreach (var relations in allocPolicy.AllocRelations)
                {
                    relations.AllocSubpolicy.MakeEmpty();

                    if (relations.AllocSubpolicy.Stakeholder != null)
                        relations.AllocSubpolicy.Stakeholder.MakeEmpty();

                    savedAllocSubpolicyIds.Add(relations.AllocSubpolicy.Id);
                }
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
                billingRelation.AllocSubpolicy = AllocSubpolicyBuilder.GetWithId(billingRelation.AllocSubpolicy.Id);
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

            obj.AllocRelations.RemoveAll(deletedRelation);
            AllocPolicyBuilder.Save(obj);
            return obj;
        }

        public string GetReportsToId()
        {
            var reportsToId = string.Empty;
            var currUserId = HttpContext.Current.User.Identity.Name;
            var currUser = StakeQuery.GetOnExpression(x => x.ExternalId == currUserId).SingleOrDefault();

            if (currUser != null && currUser.ReportingManager != Guid.Empty)
            {
                reportsToId = StakeQuery.OnIdWithAllReferences(currUser.ReportingManager).ExternalId;
            }
            return reportsToId;
        }

        #endregion

        #region Delete
        [HttpDelete]
        [HttpTransaction(Persist = true)]
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
    }
}