#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.ClientDataBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.Shared.NgGrid;
using NHibernate.Criterion;

#endregion


namespace AngularUI.FileUpload.paymentreversal
{
    public class CustomerAccounts
    {
        public string AccountNo { get; set; }
        public string CustomerName { get; set; }
    }

    public class PaymentReversalApiController : BaseApiController<Payment>
    {
        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();
        private static readonly InfoBuilder InfoBuilder = new InfoBuilder();
        private static readonly PaymentBuilder PaymentBuilder = new PaymentBuilder();

        // GET api/<controller>
        public IEnumerable<string> GetScbSystems()
        {
            return Enum.GetNames(typeof(ScbEnums.ScbSystems)).Where(x => x != ScbEnums.ScbSystems.CACS.ToString());
        }

        [HttpGet]
        
        public HttpResponseMessage GetProducts()
        {
            var data = ProductConfigBuilder.GetProducts();
            return Request.CreateResponse(HttpStatusCode.Created, data);
        }

        //GET AccountNumber for Typeahead
        [HttpGet]
        
        public HttpResponseMessage GetAccountNo(string accountNo, ScbEnums.Products products)
        {
            List<CustomerAccounts> data = InfoBuilder.OnAccNoProduct(accountNo, products)
                                                     .Select(x =>new CustomerAccounts
                                                             {
                                                                 AccountNo = x.AccountNo,
                                                                 CustomerName = x.CustomerName
                                                             }).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }


        [HttpGet]
        
        public HttpResponseMessage FetchPageData(ScbEnums.ScbSystems systems)
        {
            var data = DetachedCriteria.For(typeof(Payment));
            //data.CreateCriteria("payment.FileScheduler", "fileScheduler", JoinType.InnerJoin);
            var grid = new GridInitData(data, typeof(Payment)) { ScreenName = ColloSysEnums.GridScreenName.Payment };
            return Request.CreateResponse(HttpStatusCode.OK, grid);
        }

        [HttpPost]
        
        public HttpResponseMessage ExcludePayment(IEnumerable<Payment> payments, ScbEnums.ScbSystems systems)
        {
            foreach (var payment in payments)
            {
                payment.IsExcluded = true;
                PaymentBuilder.Save(payment);
            }
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }

        [HttpPost]
        
        public HttpResponseMessage AddPayments(Payment payment)
        {
            payment.FileDate = DateTime.Today;
            PaymentBuilder.Save(payment);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
