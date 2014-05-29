#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.BillingBuilder;
using ColloSys.QueryBuilder.GenericBuilder;

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
        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();
        private static readonly BillingPolicyBuilder BillingPolicyBuilder = new BillingPolicyBuilder();
        private static readonly BillingSubpolicyBuilder BillingSubpolicyBuilder = new BillingSubpolicyBuilder();
        private static readonly BillingRelationBuilder BillingRelationBuilder = new BillingRelationBuilder();

        #region Get

        [HttpGet]

        public HttpResponseMessage GetProducts()
        {
            var data = ProductConfigBuilder.GetProducts();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetStakeHier()
        {
            var data = Session.QueryOver<StkhHierarchy>().List();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]

        public HttpResponseMessage GetPayoutPolicy(ScbEnums.Products products, ScbEnums.Category category)
        {
            var payoutPolicy = BillingPolicyBuilder.OnProductCategory(products, category);

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

            var payoutSubpolicies = BillingSubpolicyBuilder.SubPoliciesInDb(products, category, savedPayoutSubpolicyIds).ToList();

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
            BillingPolicyBuilder.Save(obj);
            return obj;
        }

        protected override BillingPolicy BasePut(Guid id, BillingPolicy obj)
        {
            //set empty StkhPayments 
            //obj.StkhPayments = null;
            //obj.BillDetails = null;
            //obj.CollectionStkhPayments = null;
            //obj.RecoveryStkhPayments = null;
            // murge BillingRelation into added BillingPolicy

            foreach (var billingRelation in obj.BillingRelations)
            {
                billingRelation.BillingPolicy = obj;
                billingRelation.BillingSubpolicy = BillingSubpolicyBuilder.Get(billingRelation.BillingSubpolicy.Id);
                billingRelation.BillingSubpolicy.BConditions = null;
                billingRelation.BillingSubpolicy.BillDetails = null;
                billingRelation.BillingSubpolicy.BillingRelations = null;
               
                //if (billingRelation.Id == Guid.Empty)
                //{
                //    Session.SaveOrUpdate(billingRelation);
                //}
            }
            BillingRelationBuilder.Save(obj.BillingRelations);
            //BillingPolicyBuilder.Save(obj);
            return obj;
        }

        #endregion

        #region approve and reject relation

        [HttpGet]
        public HttpResponseMessage RejectRelation(Guid relationId)
        {
            var relation = BillingRelationBuilder.Get(relationId);
            BillingRelationBuilder.Delete(relation);
            return Request.CreateResponse(HttpStatusCode.OK, "");
        }

        [HttpGet]
        public HttpResponseMessage ApproveRelation(Guid relationId)
        {
            var relation = BillingRelationBuilder.Get(relationId);
            relation.Status = ColloSysEnums.ApproveStatus.Approved;
            if (relation.OrigEntityId != Guid.Empty)
            {
                var origRelation = BillingRelationBuilder.Get(relation.OrigEntityId);
                BillingRelationBuilder.Delete(origRelation);
            }
            relation.OrigEntityId = Guid.Empty;
            BillingRelationBuilder.Save(relation);
            return Request.CreateResponse(HttpStatusCode.OK, "");
        }
        #endregion
    }
}


//[HttpGet]
//
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
//
//public IEnumerable<BillingSubpolicy> GetPayoutSubpolicy(ScbEnums.Products product, ScbEnums.Category category)
//{
//    return Session.QueryOver<BillingSubpolicy>()
//                    .Where(c => c.Products == product && c.Category == category
//                        && c.PayoutSubpolicyType == ColloSysEnums.PayoutSubpolicyType.Subpolicy)
//                        .List();
//}

//[HttpGet]
//
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
