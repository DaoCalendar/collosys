#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.AllocationBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Shared;
using NHibernate.Criterion;

//using UserInterfaceAngular.NgGrid;

#endregion

//Stakeholders calls chagned
namespace UserInterfaceAngular.app
{
    public class AllocationSubPolicyApiController : BaseApiController<AllocSubpolicy>
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();
        private static readonly GKeyValueBuilder GKeyValueBuilder = new GKeyValueBuilder();
        private static readonly AllocSubpolicyBuilder AllocSubpolicyBuilder = new AllocSubpolicyBuilder();
        private static readonly AllocPolicyBuilder AllocPolicyBuilder = new AllocPolicyBuilder();
        private static readonly AllocRelationBuilder AllocRelationBuilder = new AllocRelationBuilder();
        private static readonly AllocConditionBuilder AllocConditionBuilder = new AllocConditionBuilder();

        #region Get

        [HttpGet]

        public HttpResponseMessage GetProducts()
        {
            var data = ProductConfigBuilder.GetProducts();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]

        public HttpResponseMessage GetReasons()
        {
            var data =
                GKeyValueBuilder.GetAll()
                                .Where(
                                    x => x.Area == ColloSysEnums.Activities.Allocation && x.Key == "DoNotAllocateReason")
                                .Select(x => x.Value)
                                .ToList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]

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

        public HttpResponseMessage GetStakeholders(ScbEnums.Products products)
        {
            var data = StakeQuery.OnProduct(products);
            //Stakeholders stakeholders = null;
            //StkhWorking working = null;
            //StkhHierarchy hierarchy = null;
            //var session = SessionManager.GetCurrentSession();
            //var data = session.QueryOver<Stakeholders>(() => stakeholders)
            //                  .Fetch(x => x.StkhWorkings).Eager
            //                  .JoinQueryOver(() => stakeholders.StkhWorkings, () => working)
            //                  .JoinQueryOver(() => stakeholders.Hierarchy, () => hierarchy)
            //                  .Where(() => working.Products == products)
            //                  .And(() => hierarchy.IsInAllocation)
            //                  .And(() => hierarchy.IsInField)
            //                  .TransformUsing(Transformers.DistinctRootEntity)
            //                  .List<Stakeholders>();
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

        [HttpGet]

        public HttpResponseMessage GetSubPolicy(ScbEnums.Products products, ScbEnums.Category category)
        {
            var data = AllocSubpolicyBuilder.OnProductCategory(products, category);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetConditionColumns(ScbEnums.Products products, ScbEnums.Category category)
        {
            var data = SharedViewModel.ConditionColumns(products);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }


        [HttpPost]

        public AllocRelation GetRelations(AllocSubpolicy subpolicy)
        {
            var relation = AllocRelationBuilder.OnAllocSubpolicy(subpolicy);
            if (relation == null)
            {
                var policy = AllocPolicyBuilder.NonApproved(subpolicy.Products, subpolicy.Category);
                relation = new AllocRelation
                    {
                        AllocPolicy = policy,
                        AllocSubpolicy = subpolicy
                    };
            }
            return (relation);
        }

        [HttpGet]

        public HttpResponseMessage GetConditions(Guid allocationId)
        {
            var data = AllocConditionBuilder.OnSubpolicyId(allocationId);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]

        public uint GetMaxPriority()
        {
            var data = AllocRelationBuilder.GetAll().ToList()
                                           .Select(x => x.Priority);
            return data.Any() ? data.Max() : 0;
        }


        #endregion

        #region Post

        protected override AllocSubpolicy BasePost(AllocSubpolicy obj)
        {
            foreach (var allocCondition in obj.Conditions)
            {
                allocCondition.AllocSubpolicy = obj;
            }

            //if (obj.Stakeholder.Id == Guid.Empty)
            //    obj.Stakeholder = null;
            AllocSubpolicyBuilder.Save(obj);
            return obj;
        }

        protected override AllocSubpolicy BasePut(Guid id, AllocSubpolicy obj)
        {
            foreach (var allocCondition in obj.Conditions)
            {
                allocCondition.AllocSubpolicy = obj;
            }

            if (obj.Stakeholder != null && obj.Stakeholder.Id == Guid.Empty)
                obj.Stakeholder = null;

            AllocSubpolicyBuilder.Merge(obj);
            return obj;
        }

        protected override IEnumerable<AllocSubpolicy> BaseGet()
        {
            var query = AllocSubpolicyBuilder.ApplyRelations();
            return AllocSubpolicyBuilder.Execute(query).ToList();
        }

        #endregion

        [HttpPost]

        public AllocRelation ActivateSubpolicy(AllocRelation relation)//string startDate, string endDate, AllocSubpolicy subPolicy
        {
            SetApproverId(relation);
            relation.Status = ColloSysEnums.ApproveStatus.Submitted;
            var maxpriority = GetMaxPriority();
            relation.Priority = maxpriority + 1;
            AllocRelationBuilder.Save(relation);
            return relation;
        }

        public void SetApproverId(AllocRelation relation)
        {
            var currUserId = HttpContext.Current.User.Identity.Name;
            var currUser = StakeQuery.FilterBy(x => x.ExternalId == currUserId).SingleOrDefault();

            if (currUser != null && currUser.ReportingManager != Guid.Empty)
            {
                relation.ApprovedBy = StakeQuery.OnIdWithAllReferences(currUser.Id).ExternalId;
            }
        }

        public void GetAllocPolicy(AllocSubpolicy allocSubpolicy, ScbEnums.Products products, ScbEnums.Category category)
        {
            var allocPolicy = AllocPolicyBuilder.NonApproved(products, category);

            // create new alloc policy
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