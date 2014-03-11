#region references

using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.BillingBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
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
        private static readonly ProductConfigBuilder ProductConfigBuilder=new ProductConfigBuilder();
        private static readonly BillingSubpolicyBuilder BillingSubpolicyBuilder=new BillingSubpolicyBuilder();
        private static readonly BMatrixBuilder BMatrixBuilder=new BMatrixBuilder();
        private static readonly BMatrixValueBuilder BMatrixValueBuilder=new BMatrixValueBuilder();

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
            var data = BillingSubpolicyBuilder.FormulaOnProductCategory(product, category)
                                              .Select(x => x.Name).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

         [HttpGet]
         [HttpTransaction]
        public HttpResponseMessage GetMatrix(ScbEnums.Products product, ScbEnums.Category category)
         {
             var data = BMatrixBuilder.OnProductCategory(product, category);
             return Request.CreateResponse(HttpStatusCode.OK, data);

         }

        [HttpGet]
        [HttpTransaction]
         public HttpResponseMessage GetMatrixValues(Guid matrixId)
        {
            var data = BMatrixValueBuilder.OnMatrixId(matrixId);
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
            BMatrixBuilder.Save(obj);
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
                    BMatrixValueBuilder.Save(matrixValue);
                }
                else
                {
                    BMatrixValueBuilder.Merge(matrixValue);
                }
            }

            return BMatrixBuilder.Merge(obj);
        }

        #endregion
    }
}