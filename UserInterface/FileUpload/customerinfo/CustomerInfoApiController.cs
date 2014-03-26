#region references

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.Shared.Types4Product;
using NHibernate.Criterion;

#endregion

namespace AngularUI.FileUpload.customerinfo
{
    public class CustDisplayInfo
    {
        public CustDisplayInfo()
        {
            Payments = new List<Payment>();
        }

        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public Info CustInfo { get; set; }

        public List<Payment> Payments { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }

    public class CustomerInfoApiController : BaseApiController<Info>
    {

        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();

        [HttpGet]
        
        public IEnumerable<ScbEnums.Products> GetProducts()
        {
            return ProductConfigBuilder.GetProducts();
        }

        [HttpGet]
        
        public CustDisplayInfo GetCustomerInfo(ScbEnums.Products products, string accountNo)
        {
            var infoType = ClassType.GetTypeByProductCategoryForAlloc(products, ScbEnums.Category.Liner);
            var paymentType = ClassType.GetPaymentTypeByProduct(products);

            var memberInfo = new MemberHelper<Info>();
            var custDisplayInfos = new CustDisplayInfo();

            custDisplayInfos.CustInfo = Session.CreateCriteria(infoType)
                                               .Add(Restrictions.Eq(memberInfo.GetName(x => x.AccountNo), accountNo))
                                               .List<Info>().SingleOrDefault();

            custDisplayInfos.Payments.AddRange(Session.CreateCriteria(paymentType)
                                                      .Add(Restrictions.Eq(memberInfo.GetName(x => x.AccountNo),
                                                                           accountNo))
                                                      .List<Payment>());

            return custDisplayInfos;
        }

    }

}



