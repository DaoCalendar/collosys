﻿#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;

#endregion


namespace ColloSys.UserInterface.Areas.Billing.apiController
{
    public class PayoutPolicyVm
    {
        public BillingPolicy PayoutPolicy { get; set; }

        public IList<BillingSubpolicy> UnUsedSubpolicies { get; set; }
    }

    public class PayoutPolicyApiController : BaseApiController<BillingPolicy>
    {
        #region Get

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetProducts()
        {
            var data= Session.QueryOver<ProductConfig>().Select(x => x.Product).List<ScbEnums.Products>();
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetPayoutPolicy(ScbEnums.Products products, ScbEnums.Category category)
        {
            var payoutPolicy = Session.Query<BillingPolicy>()
                                     .Where(x => x.Products == products && x.Category == category)
                                     .FetchMany(x => x.BillingRelations)
                                     .ThenFetch(r => r.BillingSubpolicy)
                                     .ThenFetch(s => s.BConditions)
                                     .SingleOrDefault();

            // create new alloc policy
            var savedPayoutSubpolicyIds = new List<Guid>();
            if (payoutPolicy == null)
            {
                payoutPolicy = new BillingPolicy() { Name = products + "_" + category, Products = products, Category = category };
            }
            else
            {
                // make alloc subpolicy empty, json serialization hack
                foreach (var relation in payoutPolicy.BillingRelations)
                {
                    relation.BillingSubpolicy.MakeEmpty();
                    savedPayoutSubpolicyIds.Add(relation.BillingSubpolicy.Id);
                }
            }

            var payoutSubpolicies = Session.QueryOver<BillingSubpolicy>()
                            .Where(x => x.PayoutSubpolicyType == ColloSysEnums.PayoutSubpolicyType.Subpolicy && x.Products == products && x.Category == category)
                            .WhereRestrictionOn(x => x.Id)
                            .Not.IsIn(savedPayoutSubpolicyIds)
                            .Fetch(x => x.BConditions).Eager
                            .Fetch(x=>x.BillingRelations).Eager
                            .TransformUsing(Transformers.DistinctRootEntity)
                            .List();
          
            var payoutPolicyVm = new PayoutPolicyVm() { PayoutPolicy = payoutPolicy, UnUsedSubpolicies = payoutSubpolicies };

            return Request.CreateResponse(HttpStatusCode.OK, payoutPolicyVm);
        }

        #endregion

        #region override Methods

        protected override BillingPolicy BasePost(BillingPolicy obj)
        {
            // murge BillingRelation into added BillingPolicy
            foreach (var billingRelation in obj.BillingRelations)
            {
                billingRelation.BillingSubpolicy = Session.Get<BillingSubpolicy>(billingRelation.BillingSubpolicy.Id);
                billingRelation.BillingPolicy = obj;
            }

            Session.SaveOrUpdate(obj);
            return obj;
        }

        protected override BillingPolicy BasePut(Guid id, BillingPolicy obj)
        {
            //set empty StkhPayments 
            //obj.StkhPayments = null;
            // murge BillingRelation into added BillingPolicy
            foreach (var billingRelation in obj.BillingRelations)
            {
                billingRelation.BillingPolicy = obj;
                billingRelation.BillingSubpolicy = Session.Get<BillingSubpolicy>(billingRelation.BillingSubpolicy.Id);

                //if (billingRelation.Id == Guid.Empty)
                //{
                //    Session.SaveOrUpdate(billingRelation);
                //}
            }

            foreach (var billingRelation in obj.BillingRelations)
            {
                Session.SaveOrUpdate(billingRelation);
            }
            Session.SaveOrUpdate(obj);
            return obj;
        }

        #endregion
    }
}


//[HttpGet]
//[HttpTransaction]
//public BillingPolicy GetPayoutPolicy(ScbEnums.Products product, ScbEnums.Category category)
//{
//    var billingPolicy = Session.QueryOver<BillingPolicy>().Where(x => x.Products == product && x.Category == category)
//                                                          .Fetch(x => x.BillingRelations).Eager
//                                                          .TransformUsing(Transformers.DistinctRootEntity)
//                                                          .SingleOrDefault()
//                     ?? new BillingPolicy() { Name = product + "_" + category, Products = product, Category = category };
//    return billingPolicy;
//}

//[HttpGet]
//[HttpTransaction]
//public IEnumerable<BillingSubpolicy> GetPayoutSubpolicy(ScbEnums.Products product, ScbEnums.Category category)
//{
//    return Session.QueryOver<BillingSubpolicy>()
//                    .Where(c => c.Products == product && c.Category == category
//                        && c.PayoutSubpolicyType == ColloSysEnums.PayoutSubpolicyType.Subpolicy)
//                        .List();
//}

//[HttpGet]
//[HttpTransaction]
//public IEnumerable<BCondition> GetBConditions(ScbEnums.Products product, ScbEnums.Category category)
//{
//    BCondition bc = null;
//    BillingSubpolicy bsp = null;
//    return Session.QueryOver<BCondition>(() => bc)
//                        .JoinAlias(() => bc.BillingSubpolicy, () => bsp)
//                        .Where(() => bsp.Products == product && bsp.Category == category
//                            && bsp.PayoutSubpolicyType == ColloSysEnums.PayoutSubpolicyType.Subpolicy)
//                        .List<BCondition>();
//}
