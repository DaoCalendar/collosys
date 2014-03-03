#region references

using System.IO;
using System.Web;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
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
        public HttpResponseMessage GetFormulaNames(ScbEnums.Products product, ScbEnums.Category category)
        {
            var data= Session.QueryOver<BillingSubpolicy>()
                            .Where(c => c.Products == product && c.Category == category
                                && c.PayoutSubpolicyType == ColloSysEnums.PayoutSubpolicyType.Formula)
                            .List().Select(c => c.Name);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetMatrixNames(ScbEnums.Products product, ScbEnums.Category category)
        {
            var data= Session.QueryOver<BMatrix>().Where(c => c.Products == product && c.Category == category)
                            .List().Select(c => c.Name);
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
            var data= Session.QueryOver<BillingSubpolicy>()
                            .Where(c => c.Products == product && c.Category == category)
                            .List();
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetBConditions(Guid parentId)
        {
            var data= Session.QueryOver<BCondition>().Where(c => c.BillingSubpolicy.Id == parentId).List();
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetFormulas(ScbEnums.Products product, ScbEnums.Category category)
        {
            var data= Session.QueryOver<BillingSubpolicy>().Where(x => x.PayoutSubpolicyType == ColloSysEnums.PayoutSubpolicyType.Formula
                                                                && x.Products == product && x.Category == category).List();
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
            var data = Session.QueryOver<BillingRelation>()
             .Select(x => x.Priority).List<uint>();
            return data.Any() ? data.Max() : 0;
        }
        #endregion

        #region "Post Method"
        [HttpPost]
        [HttpTransaction]
        public BillingRelation GetRelations(BillingSubpolicy subpolicy)
        {
            var relation = Session.QueryOver<BillingRelation>().Where(x => x.BillingSubpolicy.Id == subpolicy.Id).SingleOrDefault();
            if (relation == null)
            {
                var policy = Session.QueryOver<BillingPolicy>().Where(x => x.Products == subpolicy.Products)
                                    .And(x => x.Category == subpolicy.Category).SingleOrDefault();
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
            Session.SaveOrUpdate(relation);
            return relation;
        }

        public void SetApproverId(BillingRelation relation)
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

        #endregion

        #region override Methods

        protected override BillingSubpolicy BasePost(BillingSubpolicy obj)
        {
            foreach (var bcondition in obj.BConditions)
            {
                bcondition.BillingSubpolicy = obj;
            }

            Session.SaveOrUpdate(obj);
            return obj;
        }

        protected override BillingSubpolicy BasePut(Guid id, BillingSubpolicy obj)
        {
            foreach (var bcondition in obj.BConditions)
            {
                bcondition.BillingSubpolicy = obj;
            }

            return Session.Merge(obj);
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