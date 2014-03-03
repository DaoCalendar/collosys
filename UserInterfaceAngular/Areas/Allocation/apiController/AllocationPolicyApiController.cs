#region references

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
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using Iesi.Collections;
using Iesi.Collections.Generic;
using NHibernate;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Newtonsoft.Json.Linq;
using UserInterfaceAngular.app;

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
        #region Get

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetProducts()
        {
            var data = Session.QueryOver<ProductConfig>().Select(x => x.Product).List<ScbEnums.Products>();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetAllocPolicy(ScbEnums.Products products, ScbEnums.Category category)
        {
            AllocPolicy policy = null;
            AllocRelation relation = null;
            AllocSubpolicy subpolicy = null;
            AllocCondition condition = null;
            Stakeholders stakeholder = null;

            var allocPolicy = Session.QueryOver(() => policy)
                                     .Fetch(x => x.AllocRelations).Eager
                                     .Fetch(x => x.AllocRelations.First().AllocSubpolicy).Eager
                                     .Fetch(x => x.AllocRelations.First().AllocSubpolicy.Conditions).Eager
                                     .Fetch(x => x.AllocRelations.First().AllocSubpolicy.Stakeholder).Eager
                                     .JoinAlias(() => policy.AllocRelations, () => relation, JoinType.LeftOuterJoin)
                                     .JoinAlias(() => relation.AllocSubpolicy, () => subpolicy, JoinType.LeftOuterJoin)
                                     .JoinAlias(() => subpolicy.Conditions, () => condition, JoinType.LeftOuterJoin)
                                     .JoinAlias(() => subpolicy.Stakeholder, () => stakeholder, JoinType.LeftOuterJoin)
                                     .Where(() => policy.Products == products && policy.Category == category)

                                     .SingleOrDefault();

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

            var allocSubpolicies = Session.QueryOver<AllocSubpolicy>()
                            .Where(x => x.Products == products && x.Category == category)
                            .WhereRestrictionOn(x => x.Id)
                            .Not.IsIn(savedAllocSubpolicyIds)
                            .Fetch(x => x.Stakeholder).Eager
                            .Fetch(x => x.Conditions).Eager
                            .TransformUsing(Transformers.DistinctRootEntity)
                            .List();

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
                billingRelation.AllocSubpolicy = Session.Get<AllocSubpolicy>(billingRelation.AllocSubpolicy.Id);
                billingRelation.AllocPolicy = obj;
            }

            Session.SaveOrUpdate(obj);
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

                var delrelation = Session.Load<AllocRelation>(billingRelation.OrigEntityId);
                deletedRelation.Add(delrelation);
                billingRelation.OrigEntityId = Guid.Empty;
            }

            foreach (var billingRelation in obj.AllocRelations)
            {
                if (deletedRelation.Any(x => x.Id == billingRelation.Id))
                {
                    Session.Delete(billingRelation);
                    continue;
                }
                Session.SaveOrUpdate(billingRelation);
            }

            obj.AllocRelations.RemoveAll(deletedRelation);

            Session.SaveOrUpdate(obj);
            return obj;
        }

        public string GetReportsToId()
        {
            var reportsToId = string.Empty;
            var currUserId = HttpContext.Current.User.Identity.Name;
            var currUser =
                Session.QueryOver<Stakeholders>()
                        .Where(x => x.ExternalId == currUserId).SingleOrDefault();

            if (currUser != null && currUser.ReportingManager != Guid.Empty)
            {
                reportsToId =
                    Session.QueryOver<Stakeholders>()
                            .Where(x => x.Id == currUser.ReportingManager).SingleOrDefault().ExternalId;

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

                var relation = Session.Load<AllocRelation>(id);
                Session.Delete(relation);

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