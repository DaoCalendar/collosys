#region references

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.BillingBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Shared;
using NHibernate.Criterion;

#endregion

namespace AngularUI.Billing.subpolicy
{
    public class PayoutSubpolicyApiController : BaseApiController<BillingSubpolicy>
    {
        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        private static readonly BillingSubpolicyBuilder BillingSubpolicyBuilder = new BillingSubpolicyBuilder();
        private static readonly BMatrixBuilder BMatrixBuilder = new BMatrixBuilder();
        private static readonly BillingRelationBuilder BillingRelationBuilder = new BillingRelationBuilder();
        private static readonly BillingPolicyBuilder BillingPolicyBuilder = new BillingPolicyBuilder();

        #region Get

        [HttpGet]
        public HttpResponseMessage GetProducts()
        {
            var data = ProductConfigBuilder.GetProducts();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetFormulaNames(ScbEnums.Products product)
        {
            var data = BillingSubpolicyBuilder
                .FormulaOnProductCategory(product)
                .Select(c => c.Name)
                .ToList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetMatrixNames(ScbEnums.Products product)
        {
            var data = BMatrixBuilder.OnProductCategory(product)
                                     .Select(x => x.Name).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetMatrixList(ScbEnums.Products product)
        {
            var data = BMatrixBuilder.OnProductCategory(product)
                .ToList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet] // action does not required DB Transaction
        public HttpResponseMessage GetColumnNames(ScbEnums.Products product)
        {
            var data = SharedViewModel.ConditionColumns(product).Where(c => c.InputType == ColloSysEnums.HtmlInputType.number).Select(c => c.field);
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

        [HttpGet] // action does not required DB Transaction
        public HttpResponseMessage GetColumns()
        {
            var data = SharedViewModel.BillingServiceConditionColumns();  //SharedViewModel.ConditionColumns(product, category);
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

        [HttpGet]

        public HttpResponseMessage GetPayoutSubpolicy(ScbEnums.Products product, ColloSysEnums.PolicyType policyType)
        {
            //  var data = BillingSubpolicyBuilder.OnProductCategory(product, policyType);  **commmented By SONu (as per mahendra told)
            return Request.CreateResponse(HttpStatusCode.OK, "");
        }

        [HttpGet]

        public HttpResponseMessage GetBConditions(Guid parentId)
        {
            //  var data = BConditionBuilder.OnSubpolicyId(parentId);  ** commented by SONU (as per mahendra told)
            return Request.CreateResponse(HttpStatusCode.OK, "");
        }

        //[CamelCasedApiMethod]
        [HttpGet]

        public HttpResponseMessage GetFormulas(ScbEnums.Products product)
        {
            var data = BillingSubpolicyBuilder.FormulaOnProductCategory(product);

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

        public uint GetMaxPriority()
        {
            var data = BillingRelationBuilder.GetAll().Select(x => x.Priority).ToList();
            return data.Any() ? data.Max() : 0;
        }
        #endregion

        #region Post Method
        [HttpPost]
        public BillingRelation GetRelations(BillingSubpolicy subpolicy)
        {
            var relation = BillingRelationBuilder.OnSubpolicyId(subpolicy.Id);
            if (relation == null)
            {
                var policy = BillingPolicyBuilder.OnProductCategory(subpolicy.Products);
                policy.BillDetails = null;
                policy.BillingRelations = null;
                relation = new BillingRelation
                {
                    BillingPolicy = policy,
                    BillingSubpolicy = subpolicy
                };
            }

            return (relation);
        }

        [HttpPost]

        public BillingRelation ActivateSubpolicy(BillingRelation relation)//string startDate, string endDate, BillingSubpolicy subPolicy
        {
            SetApproverId(relation);
            relation.Status = ColloSysEnums.ApproveStatus.Submitted;
            var maxpriority = GetMaxPriority();
            relation.Priority = maxpriority + 1;
            BillingRelationBuilder.Save(relation);
            return relation;
        }

        private void SetApproverId(BillingRelation relation)
        {
            var currUserId = HttpContext.Current.User.Identity.Name;
            try
            {
                var currUser = StakeQuery.FilterBy(x => x.ExternalId == currUserId).SingleOrDefault();
                if (currUser != null && currUser.ReportingManager != Guid.Empty)
                {
                    relation.ApprovedBy = StakeQuery.OnIdWithAllReferences(currUser.ReportingManager).ExternalId;
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }
        }

        #endregion

        #region override Methods

        protected override BillingSubpolicy BasePost(BillingSubpolicy obj)
        {
            foreach (var bcondition in obj.BillTokens)
            {
                bcondition.BillingSubpolicy = obj;
            }
            BillingSubpolicyBuilder.Save(obj);
            return obj;
        }

        protected override BillingSubpolicy BasePut(Guid id, BillingSubpolicy obj)
        {
            //foreach (var bcondition in obj.BConditions)  **commented by SONU (as per mahendra told)
            //{
            //    bcondition.BillingSubpolicy = obj;
            //}

            BillingSubpolicyBuilder.Merge(obj);
            return obj;
        }

        #endregion
    }
}