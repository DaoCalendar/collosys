#region references

using System.IO;
using System.Web;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.BillingBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Criterion;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

#endregion

namespace ColloSys.UserInterface.Areas.Billing.apiController
{
    public class PayoutSubpolicyApiController : BaseApiController<BillingSubpolicy>
    {
        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();
        private static readonly StakeQueryBuilder StakeQuery=new StakeQueryBuilder();
        private static readonly BillingSubpolicyBuilder BillingSubpolicyBuilder=new BillingSubpolicyBuilder();
        private static readonly BMatrixBuilder BMatrixBuilder=new BMatrixBuilder();
        private static readonly BConditionBuilder BConditionBuilder=new BConditionBuilder();
        private static readonly BillingRelationBuilder BillingRelationBuilder=new BillingRelationBuilder();
        private static readonly BillingPolicyBuilder BillingPolicyBuilder=new BillingPolicyBuilder();

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
        public HttpResponseMessage GetFormulaNames(ScbEnums.Products product, ScbEnums.Category category)
        {
            var data=BillingSubpolicyBuilder
                .FormulaOnProductCategory(product,category)
                .Select(c => c.Name)
                .ToList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetMatrixNames(ScbEnums.Products product, ScbEnums.Category category)
        {
            var data = BMatrixBuilder.OnProductCategory(product, category)
                                     .Select(x => x.Name).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet] // action does not required DB Transaction
        public HttpResponseMessage GetColumnNames(ScbEnums.Products product, ScbEnums.Category category)
        {
            var data= SharedViewModel.ConditionColumns(product, category).Where(c => c.InputType == ColloSysEnums.HtmlInputType.number).Select(c => c.field);
            return Request.CreateResponse(HttpStatusCode.OK, data);
            
        }

        [HttpGet] // action does not required DB Transaction
        public HttpResponseMessage GetColumns(ScbEnums.Products product, ScbEnums.Category category)
        {
            var data = SharedViewModel.BillingServiceConditionColumns();  //SharedViewModel.ConditionColumns(product, category);
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetPayoutSubpolicy(ScbEnums.Products product, ScbEnums.Category category)
        {
            var data = BillingSubpolicyBuilder.OnProductCategory(product, category);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetBConditions(Guid parentId)
        {
            var data = BConditionBuilder.OnSubpolicyId(parentId);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetFormulas(ScbEnums.Products product, ScbEnums.Category category)
        {
            var data = BillingSubpolicyBuilder.FormulaOnProductCategory(product, category);
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
        public uint GetMaxPriority()
        {
            var data = BillingRelationBuilder.GetAll().Select(x => x.Priority).ToList();
            return data.Any() ? data.Max() : 0;
        }
        #endregion

        #region "Post Method"
        [HttpPost]
        [HttpTransaction]
        public BillingRelation GetRelations(BillingSubpolicy subpolicy)
        {
            var relation = BillingRelationBuilder.OnSubpolicyId(subpolicy.Id);
            if (relation == null)
            {
                var policy = BillingPolicyBuilder.OnProductCategory(subpolicy.Products, subpolicy.Category);
                relation = new BillingRelation
                {
                    BillingPolicy = policy,
                    BillingSubpolicy = subpolicy
                };
            }

            return (relation);
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public BillingRelation ActivateSubpolicy(BillingRelation relation)//string startDate, string endDate, BillingSubpolicy subPolicy
        {
            SetApproverId(relation);
            relation.Status = ColloSysEnums.ApproveStatus.Submitted;
            var maxpriority = GetMaxPriority();
            relation.Priority =(uint) maxpriority + 1;
            BillingRelationBuilder.Save(relation);
            return relation;
        }

        public void SetApproverId(BillingRelation relation)
        {
            var currUserId = HttpContext.Current.User.Identity.Name;
            var currUser = StakeQuery.FilterBy(x => x.ExternalId == currUserId).SingleOrDefault();

            if (currUser != null && currUser.ReportingManager != Guid.Empty)
            {
                relation.ApprovedBy = StakeQuery.OnIdWithAllReferences(currUser.ReportingManager).ExternalId;
            }
        }

        #endregion

        #region override Methods

        protected override BillingSubpolicy BasePost(BillingSubpolicy obj)
        {
            foreach (var bcondition in obj.BConditions)
            {
                bcondition.BillingSubpolicy = obj;
            }
            BillingSubpolicyBuilder.Save(obj);
            return obj;
        }

        protected override BillingSubpolicy BasePut(Guid id, BillingSubpolicy obj)
        {
            foreach (var bcondition in obj.BConditions)
            {
                bcondition.BillingSubpolicy = obj;
            }

            BillingSubpolicyBuilder.Merge(obj);
            return obj;
        }

        #endregion

        //#region Delete
        //[HttpDelete]
        //[HttpTransaction(Persist = true)]
        //public virtual HttpResponseMessage RejectSubpolicy(Guid id)
        //{
        //    try
        //    {
        //        if (id.Equals(Guid.Empty))
        //        {
        //            throw new InvalidDataException("Id provided is empty. No entity with such id exist.");
        //        }

        //        var relation = Session.Load<BillingRelation>(id);
        //        Session.Delete(relation);

        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
        //    }
        //}
        //#endregion
    }
}