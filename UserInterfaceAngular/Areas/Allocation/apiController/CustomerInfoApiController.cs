using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Providers.Entities;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.Shared.Types4Product;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Criterion;

namespace ColloSys.UserInterface.Areas.Allocation.apiController
{
    public class CustDisplayInfo
    {
        public CustDisplayInfo()
        {
            Payments = new List<SharedPayment>();
        }

        public SharedInfo CustInfo { get; set; }

        public List<SharedPayment> Payments { get; set; }
    }

    public class CustomerInfoApiController : BaseApiController<CInfo>
    {
        [HttpGet]
        [HttpTransaction]
        public IEnumerable<ScbEnums.Products> GetProducts()
        {
            return Session.QueryOver<ProductConfig>().Select(x => x.Product).List<ScbEnums.Products>();
        }

        [HttpGet]
        [HttpTransaction]
        public CustDisplayInfo GetCustomerInfo(ScbEnums.Products products, string accountNo)
        {
            var infoType = ClassType.GetTypeByProductCategoryForAlloc(products, ScbEnums.Category.Liner);
            var paymentType = ClassType.GetPaymentTypeByProduct(products);

            var memberInfo = new MemberHelper<SharedInfo>();
            var custDisplayInfos = new CustDisplayInfo();

            custDisplayInfos.CustInfo = Session.CreateCriteria(infoType)
                                               .Add(Restrictions.Eq(memberInfo.GetName(x => x.AccountNo), accountNo))
                                               .List<SharedInfo>().SingleOrDefault();

            custDisplayInfos.Payments.AddRange(Session.CreateCriteria(paymentType)
                                                      .Add(Restrictions.Eq(memberInfo.GetName(x => x.AccountNo),
                                                                           accountNo))
                                                      .List<SharedPayment>());

            return custDisplayInfos;
        }

    }

}



