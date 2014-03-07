#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.Shared.NgGrid;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Criterion;
using NHibernate.Linq;

#endregion


namespace ColloSys.UserInterface.Areas.FileUploader.apiController
{
    public class CustomerAccounts
    {
        public string AccountNo { get; set; }
        public string CustomerName { get; set; }
    }

    public class PaymentReversalApiController : BaseApiController<Payment>
    {
        private static readonly ProductConfigBuilder ProductConfigBuilder=new ProductConfigBuilder();

        // GET api/<controller>
        public IEnumerable<string> GetScbSystems()
        {
            return Enum.GetNames(typeof(ScbEnums.ScbSystems)).Where(x => x != ScbEnums.ScbSystems.CACS.ToString());
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetProducts()
        {
            var data = ProductConfigBuilder.GetProducts();
            return Request.CreateResponse(HttpStatusCode.Created, data);
        }

        //GET AccountNumber for Typeahead
        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetAccountNo(string accountNo, ScbEnums.Products products)
        {
            List<CustomerAccounts> data = Session.Query<Info>()
                               .Where(x => x.AccountNo.ToString()
                                            .StartsWith(accountNo) && x.Product == products)
                               .Take(10).Select(x => new CustomerAccounts { AccountNo = x.AccountNo, CustomerName = x.CustomerName })
                               .ToList()
                               .Distinct().ToList();
          return Request.CreateResponse(HttpStatusCode.OK, data);
        }

       
        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage FetchPageData(ScbEnums.ScbSystems systems)
        {
            return Request.CreateResponse(HttpStatusCode.OK, GetRPaymentData());
        }


        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage ExcludePayment(IEnumerable<Payment> payments, ScbEnums.ScbSystems systems)
        {
            foreach (var payment in payments)
            {
                payment.IsExcluded = true;
                Session.SaveOrUpdate(payment);
            }
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }

        private static GridInitData GetRPaymentData()
        {
            var data = DetachedCriteria.For(typeof(Payment));
            //data.CreateCriteria("payment.FileScheduler", "fileScheduler", JoinType.InnerJoin);
            var grid= new GridInitData(data, typeof(Payment));
            grid.ScreenName=ColloSysEnums.GridScreenName.Payment;
            return grid;
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage AddPayments(Payment payment)
        {
            payment.FileDate = DateTime.Today;
            Session.SaveOrUpdate(payment);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}

//switch (products)
//{
//    case ScbEnums.Products.CC:
//        data = Session.Query<Info>()
//                 .Where(x => x.AccountNo.ToString()
//                                         .StartsWith(accountNo) && x.Product == products)
//                 .Take(10).Select(x => new CustomerAccounts { AccountNo = x.AccountNo, CustomerName = x.CustomerName })
//                 .ToList()
//                 .Distinct().ToList();
//        return Request.CreateResponse(HttpStatusCode.OK, data);
//    case ScbEnums.Products.AUTO_OD:
//    case ScbEnums.Products.SMC:

//    case ScbEnums.Products.AUTO:
//    case ScbEnums.Products.MORT:
//    case ScbEnums.Products.PL:
//    case ScbEnums.Products.SME_BIL:
//    case ScbEnums.Products.SME_LAP:
//    case ScbEnums.Products.SME_LAP_OD:
//    case ScbEnums.Products.SME_ME:
//        data = Session.Query<Info>()
//                  .Where(x => x.AccountNo.ToString()
//                                         .StartsWith(accountNo) && x.Product == products)
//                 .Take(10).Select(x => new CustomerAccounts { AccountNo = x.AccountNo, CustomerName = x.CustomerName })
//                 .ToList()
//                 .Distinct().ToList();
//        return Request.CreateResponse(HttpStatusCode.OK, data);
//    default:
//        throw new ArgumentOutOfRangeException("products");
//}



//switch (systems)
//{
//    case ScbEnums.ScbSystems.CCMS:
//        return Request.CreateResponse(HttpStatusCode.OK, GetRPaymentData<Payment>());
//    case ScbEnums.ScbSystems.EBBS:
//        return Request.CreateResponse(HttpStatusCode.OK, GetRPaymentData<Payment>());
//    case ScbEnums.ScbSystems.RLS:
//        return Request.CreateResponse(HttpStatusCode.OK, GetRPaymentData<Payment>());
//    default:
//        throw new ArgumentOutOfRangeException("systems");
//}


//switch (sharedPayment.Products)
//{
//    case ScbEnums.Products.CC:
//        var cPayments = payment.ToObject<Payment>();
//        cPayments.FileDate = DateTime.Today;
//        Session.SaveOrUpdate(cPayments);
//        break;
//    case ScbEnums.Products.AUTO_OD:
//    case ScbEnums.Products.SMC:
//        var ePayments = payment.ToObject<Payment>();
//        ePayments.FileDate = DateTime.Today;
//        Session.SaveOrUpdate(ePayments);
//        break;
//    case ScbEnums.Products.AUTO:
//    case ScbEnums.Products.MORT:
//    case ScbEnums.Products.PL:
//    case ScbEnums.Products.SME_BIL:
//    case ScbEnums.Products.SME_LAP:
//    case ScbEnums.Products.SME_LAP_OD:
//    case ScbEnums.Products.SME_ME:
//        var rPayments = payment.ToObject<Payment>();
//        rPayments.FileDate = DateTime.Today;
//        Session.SaveOrUpdate(rPayments);
//        break;

//    default:
//        throw new ArgumentOutOfRangeException("products");
//}




//switch (systems)
//{
//    case ScbEnums.ScbSystems.CCMS:
//        var cPayments = payments.Select(x => x.ToObject<Payment>());


//        break;
//    case ScbEnums.ScbSystems.EBBS:
//        var ePayments = payments.Select(x => x.ToObject<Payment>());

//        if (ePayments == null)
//            break;

//        foreach (var payment in ePayments)
//        {
//            payment.IsExcluded = true;
//            Session.SaveOrUpdate(payment);
//        }
//        break;
//    case ScbEnums.ScbSystems.RLS:
//        var rPayments = payments.Select(x => x.ToObject<Payment>());

//        if (rPayments == null)
//            break;

//        foreach (var payment in rPayments)
//        {
//            payment.IsExcluded = true;
//            Session.SaveOrUpdate(payment);
//        }
//        break;
//    default:
//        throw new ArgumentOutOfRangeException("systems");
//}