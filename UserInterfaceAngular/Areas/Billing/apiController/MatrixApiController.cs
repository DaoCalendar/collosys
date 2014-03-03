#region references

using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.UserInterface.Shared.Attributes;

#endregion


namespace ColloSys.UserInterface.Areas.Billing.apiController
{
    public class MatrixApiController : BaseApiController<BMatrix>
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
        public HttpResponseMessage GetMatrix(ScbEnums.Products product, ScbEnums.Category category)
         {
             var data= Session.QueryOver<BMatrix>().Where(c => c.Products == product && c.Category == category).List();
             return Request.CreateResponse(HttpStatusCode.OK, data);

         }

        [HttpGet]
        [HttpTransaction]
         public HttpResponseMessage GetMatrixValues(Guid matrixId)
        {
            var data= Session.QueryOver<BMatrixValue>()
                            .Where(c => c.BMatrix.Id == matrixId)
                            .List();
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

        [HttpGet] // action does not required DB Transaction
        public HttpResponseMessage GetColumnNames(ScbEnums.Products product, ScbEnums.Category category)
        {
            var data = SharedViewModel.BillingServiceConditionColumns();//.Select(x=>x.field);  //SharedViewModel.ConditionColumns(product, category).Select(c => c.field);
                                    //.Where(c => c.InputType == ColloSysEnums.HtmlInputType.number).Select(c => c.field);
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }



        #endregion

        #region override methods

        protected override BMatrix BasePost(BMatrix obj)
        {
            // murge MatrixValue into added Matrix         
            foreach (var matrixValue in obj.BMatricesValues)
            {
                matrixValue.BMatrix = obj;
            }

            Session.SaveOrUpdate(obj);
            return obj;
        }

        protected override BMatrix BasePut(Guid id, BMatrix obj)
        {
            // murge MatrixValue into added Matrix
            foreach (var matrixValue in obj.BMatricesValues)
            {
                matrixValue.BMatrix = obj;

                if (matrixValue.Id == Guid.Empty)
                {
                    Session.SaveOrUpdate(matrixValue);
                }
                else
                {
                    Session.Merge(matrixValue);
                }
            }

            return Session.Merge(obj);
        }

        #endregion
    }
}