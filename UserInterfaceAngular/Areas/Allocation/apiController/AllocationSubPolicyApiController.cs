#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Newtonsoft.Json.Linq;
using UserInterfaceAngular.NgGrid;

#endregion

namespace UserInterfaceAngular.app
{
    public class AllocationSubPolicyApiController : BaseApiController<AllocSubpolicy>
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
        public HttpResponseMessage GetReasons()
        {
            var data = Session.QueryOver<GKeyValue>()
                       .Where(x => x.Area == ColloSysEnums.Activities.Allocation && x.Key == "DoNotAllocateReason")
                       .Select(x => x.Value)
                       .List<string>();

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetValuesofColumn(string columnName)
        {
            var column = columnName.Split('.');

            if (column.Length < 2)
                return null;

            var className = column[0];
            var properyName = column[1];

            if (string.IsNullOrWhiteSpace(className) || string.IsNullOrWhiteSpace(properyName))
                return null;

            var classType = typeof(CLiner).Assembly.ExportedTypes.SingleOrDefault(x => x.Name == className);

            if (classType == null)
                return null;

            var property = classType.GetProperty(properyName);
            if (property == null)
                return null;

            var query = DetachedCriteria.For(classType);
            query.SetProjection(Projections.Distinct(Projections.Property(property.Name)));
            var execCriteria = query.GetExecutableCriteria(Session);
            var data = execCriteria.List<string>();
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetStakeholders(ScbEnums.Products products)
        {
            Stakeholders stakeholders = null;
            StkhWorking working = null;
            StkhHierarchy hierarchy = null;
            var session = SessionManager.GetCurrentSession();
            var data = session.QueryOver<Stakeholders>(() => stakeholders)
                              .Fetch(x => x.StkhWorkings).Eager
                              .JoinQueryOver(() => stakeholders.StkhWorkings, () => working)
                              .JoinQueryOver(() => stakeholders.Hierarchy, () => hierarchy)
                              .Where(() => working.Products == products)
                              .And(() => hierarchy.IsInAllocation)
                              .And(() => hierarchy.IsInField)
                              .TransformUsing(Transformers.DistinctRootEntity)
                              .List<Stakeholders>();
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetSubPolicy(ScbEnums.Products products, ScbEnums.Category category)
        {
            var data = Session.QueryOver<AllocSubpolicy>().Fetch(x => x.Stakeholder).Eager
                .Where(x => x.Products == products && x.Category == category).List();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetConditionColumns(ScbEnums.Products products, ScbEnums.Category category)
        {
            var data = SharedViewModel.ConditionColumns(products, category);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }



        [HttpPost]
        [HttpTransaction]
        public AllocRelation GetRelations(AllocSubpolicy subpolicy)
        {
            var relation = Session.QueryOver<AllocRelation>().Where(x => x.AllocSubpolicy.Id == subpolicy.Id).SingleOrDefault();
            if (relation == null)
            {
                var policy = Session.QueryOver<AllocPolicy>().Where(x => x.Products == subpolicy.Products)
                                    .And(x => x.Category == subpolicy.Category).SingleOrDefault();
                relation = new AllocRelation
                    {
                        AllocPolicy = policy,
                        AllocSubpolicy = subpolicy
                    };
            }

            return (relation);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetConditions(Guid allocationId)
        {
            var data = Session.QueryOver<AllocCondition>().Where(x => x.AllocSubpolicy.Id == allocationId).List();
            AllocPolicy policy = null;
            AllocRelation relation = null;
            AllocSubpolicy subpolicy = null;
            AllocCondition condition = null;
            Stakeholders stakeholder = null;
            //var subpolicyData = Session.QueryOver<AllocSubpolicy>().Where(x => x.Id == allocationId).SingleOrDefault();
            var subpolicyData2 = Session.QueryOver<AllocSubpolicy>(() => subpolicy)
                                        .Fetch(x => x.AllocRelations).Eager
                                        .JoinAlias(() => subpolicy.AllocRelations, () => relation,
                                                   JoinType.LeftOuterJoin)
                                        .Where(() => subpolicy.Id == allocationId)
                                        .SingleOrDefault();
            //var allocPolicy = Session.QueryOver(() => policy)
            //                         .Fetch(x => x.AllocRelations).Eager
            //                         .Fetch(x => x.AllocRelations.First().AllocSubpolicy).Eager
            //                         .Fetch(x => x.AllocRelations.First().AllocSubpolicy.Conditions).Eager
            //                         .Fetch(x => x.AllocRelations.First().AllocSubpolicy.Stakeholder).Eager
            //                         .JoinAlias(() => policy.AllocRelations, () => relation, JoinType.LeftOuterJoin)
            //                         .JoinAlias(() => relation.AllocSubpolicy, () => subpolicy, JoinType.LeftOuterJoin)
            //                         .JoinAlias(() => subpolicy.Conditions, () => condition, JoinType.LeftOuterJoin)
            //                         .JoinAlias(() => subpolicy.Stakeholder, () => stakeholder, JoinType.LeftOuterJoin)
            //                         .Where(() => policy.Products == subpolicyData.Products && policy.Category == subpolicyData.Category)

            //                         .SingleOrDefault();


            //// create new alloc policy
            //var savedAllocSubpolicyIds = new List<Guid>();
            //if (allocPolicy == null)
            //{
            //    allocPolicy = new AllocPolicy() { Name = products + "_" + category, Products = products, Category = category };
            //}
            //else
            //{
            //    // make alloc subpolicy empty, json serialization hack
            //    foreach (var relations in allocPolicy.AllocRelations)
            //    {
            //        relations.AllocSubpolicy.MakeEmpty();

            //        if (relations.AllocSubpolicy.Stakeholder != null)
            //            relations.AllocSubpolicy.Stakeholder.MakeEmpty();

            //        savedAllocSubpolicyIds.Add(relations.AllocSubpolicy.Id);
            //    }
            //}



            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public uint GetMaxPriority()
        {
            var data = Session.QueryOver<AllocRelation>()
             .Select(x => x.Priority).List<uint>();
            return data.Any()?data.Max():0;
        }


        #endregion

        #region Post

        protected override AllocSubpolicy BasePost(AllocSubpolicy obj)
        {
            foreach (var allocCondition in obj.Conditions)
            {
                allocCondition.AllocSubpolicy = obj;
            }

            if (obj.Stakeholder.Id == Guid.Empty)
                obj.Stakeholder = null;

            Session.SaveOrUpdate(obj);
            return obj;
        }



        protected override AllocSubpolicy BasePut(Guid id, AllocSubpolicy obj)
        {
            foreach (var allocCondition in obj.Conditions)
            {
                allocCondition.AllocSubpolicy = obj;
            }

            if (obj.Stakeholder.Id == Guid.Empty)
                obj.Stakeholder = null;

            obj = Session.Merge(obj);
            return obj;
        }

        protected override IEnumerable<AllocSubpolicy> BaseGet()
        {
            return Session.QueryOver<AllocSubpolicy>()
                          .Fetch(x => x.Stakeholder).Eager
                          .List();
        }

        #endregion

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public AllocRelation ActivateSubpolicy(AllocRelation relation)//string startDate, string endDate, AllocSubpolicy subPolicy
        {

            //var relation = Session.QueryOver<AllocRelation>().Where(x => x.AllocSubpolicy.Id == subPolicy.Id).SingleOrDefault();
            //if (relation == null)
            //{
            //    var policy = Session.QueryOver<AllocPolicy>().Where(x => x.Products == subPolicy.Products)
            //                        .And(x => x.Category == subPolicy.Category).SingleOrDefault();
            //    relation = new AllocRelation
            //    {
            //        AllocPolicy = policy,
            //        AllocSubpolicy = subPolicy
            //    };
            //}


            SetApproverId(relation);
            relation.Status = ColloSysEnums.ApproveStatus.Submitted;
            var maxpriority = GetMaxPriority();
            relation.Priority = maxpriority + 1;
            Session.SaveOrUpdate(relation);
            return relation;
        }

        public void SetApproverId(AllocRelation relation)
        {
            var currUserId = HttpContext.Current.User.Identity.Name;
            var currUser =
                Session.QueryOver<Stakeholders>()
                        .Where(x => x.ExternalId == currUserId).SingleOrDefault();

            if (currUser != null && currUser.ReportingManager != Guid.Empty)
            {
                var reportsToUserId =
                    Session.QueryOver<Stakeholders>()
                            .Where(x => x.Id == currUser.ReportingManager).SingleOrDefault().ExternalId;
                relation.ApprovedBy = reportsToUserId;
            }
        }

        public void GetAllocPolicy(AllocSubpolicy allocSubpolicy, ScbEnums.Products products, ScbEnums.Category category)
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
                allocPolicy = new AllocPolicy()
                    {
                        Name = products + "_" + category,
                        Products = products,
                        Category = category
                    };

                var subpolicyRelation = new AllocRelation() { AllocSubpolicy = allocSubpolicy, AllocPolicy = allocPolicy };
                allocPolicy.AllocRelations.Add(subpolicyRelation);
            }
            else
            {
                var subpolicyRelation = allocPolicy.AllocRelations.SingleOrDefault(x => x.AllocSubpolicy.Id == allocSubpolicy.Id);
                if (subpolicyRelation == null)
                {
                    subpolicyRelation = new AllocRelation() { AllocSubpolicy = allocSubpolicy, AllocPolicy = allocPolicy };
                }

                // set start date and end date
            }
        }
    }
}