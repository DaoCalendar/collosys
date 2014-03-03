using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.Shared.NgGrid;
using ColloSys.UserInterface.Areas.FileUploader.Models;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using Newtonsoft.Json.Linq;

namespace ColloSys.UserInterface.Areas.FileUploader.apiController
{
    public class CustomerAccounts
    {
        public string AccountNo { get; set; }
        public string CustomerName { get; set; }
    }

    public class PaymentReversalApiController : BaseApiController<RPayment>
    {
        // GET api/<controller>
        public IEnumerable<string> GetScbSystems()
        {
            return Enum.GetNames(typeof(ScbEnums.ScbSystems)).Where(x => x != ScbEnums.ScbSystems.CACS.ToString());
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetProducts()
        {
            var data = Session.QueryOver<ProductConfig>().Select(x => x.Product).List<ScbEnums.Products>();
            return Request.CreateResponse(HttpStatusCode.Created, data);
        }

        //GET AccountNumber for Typeahead
        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetAccountNo(string accountNo, ScbEnums.Products products)
        {
          var data = new List<CustomerAccounts>();
            switch (products)
            {
                case ScbEnums.Products.CC:
                    data = Session.Query<CInfo>()
                             .Where(x => x.AccountNo.ToString()
                                                     .StartsWith(accountNo) && x.Product == products)
                             .Take(10).Select(x => new CustomerAccounts { AccountNo = x.AccountNo, CustomerName = x.CustomerName })
                             .ToList()
                             .Distinct().ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, data);
                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SMC:
                    data = Session.Query<EInfo>()
                             .Where(x => x.AccountNo.ToString()
                                                     .StartsWith(accountNo) && x.Product == products)
                             .Take(10).Select(x => new CustomerAccounts { AccountNo = x.AccountNo, CustomerName = x.CustomerName })
                             .ToList()
                             .Distinct().ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, data);
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.MORT:
                case ScbEnums.Products.PL:
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.SME_LAP_OD:
                case ScbEnums.Products.SME_ME:
                    data = Session.Query<RInfo>()
                              .Where(x => x.AccountNo.ToString()
                                                     .StartsWith(accountNo) && x.Product == products)
                             .Take(10).Select(x => new CustomerAccounts { AccountNo = x.AccountNo, CustomerName = x.CustomerName })
                             .ToList()
                             .Distinct().ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, data);
                default:
                    throw new ArgumentOutOfRangeException("products");
            }

        }

       
        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage FetchPageData(ScbEnums.ScbSystems systems)
        {
            switch (systems)
            {
                case ScbEnums.ScbSystems.CCMS:
                    return Request.CreateResponse(HttpStatusCode.OK, GetRPaymentData<CPayment>());
                case ScbEnums.ScbSystems.EBBS:
                    return Request.CreateResponse(HttpStatusCode.OK, GetRPaymentData<EPayment>());
                case ScbEnums.ScbSystems.RLS:
                    return Request.CreateResponse(HttpStatusCode.OK, GetRPaymentData<RPayment>());
                default:
                    throw new ArgumentOutOfRangeException("systems");
            }
        }


        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage ExcludePayment(IEnumerable<JObject> payments, ScbEnums.ScbSystems systems)
        {
            switch (systems)
            {
                case ScbEnums.ScbSystems.CCMS:
                    var cPayments = payments.Select(x => x.ToObject<CPayment>());

                    foreach (var payment in cPayments)
                    {
                        payment.IsExcluded = true;
                        Session.SaveOrUpdate(payment);
                    }
                    break;
                case ScbEnums.ScbSystems.EBBS:
                    var ePayments = payments.Select(x => x.ToObject<EPayment>());

                    if (ePayments == null)
                        break;

                    foreach (var payment in ePayments)
                    {
                        payment.IsExcluded = true;
                        Session.SaveOrUpdate(payment);
                    }
                    break;
                case ScbEnums.ScbSystems.RLS:
                    var rPayments = payments.Select(x => x.ToObject<RPayment>());

                    if (rPayments == null)
                        break;

                    foreach (var payment in rPayments)
                    {
                        payment.IsExcluded = true;
                        Session.SaveOrUpdate(payment);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("systems");
            }

            return Request.CreateResponse(HttpStatusCode.OK, true);
        }

        private static GridInitData GetRPaymentData<T>()
        {
            var data = DetachedCriteria.For<T>("payment");
            //data.CreateCriteria("payment.FileScheduler", "fileScheduler", JoinType.InnerJoin);
            var grid= new GridInitData(data, typeof(T));
            grid.ScreenName=ColloSysEnums.GridScreenName.Payment;
            return grid;
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage AddPayments(JObject payment)
        {
            var sharedPayment = payment.ToObject<CPayment>();

            switch (sharedPayment.Products)
            {
                case ScbEnums.Products.CC:
                    var cPayments = payment.ToObject<CPayment>();
                    cPayments.FileDate = DateTime.Today;
                    Session.SaveOrUpdate(cPayments);
                    break;
                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SMC:
                    var ePayments = payment.ToObject<EPayment>();
                    ePayments.FileDate = DateTime.Today;
                    Session.SaveOrUpdate(ePayments);
                    break;
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.MORT:
                case ScbEnums.Products.PL:
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.SME_LAP_OD:
                case ScbEnums.Products.SME_ME:
                    var rPayments = payment.ToObject<RPayment>();
                    rPayments.FileDate = DateTime.Today;
                    Session.SaveOrUpdate(rPayments);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("products");
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}