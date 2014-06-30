using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.AllocationBuilder;
using NHibernate.Linq;

namespace AngularUI.Allocation.distributionpercentage
{
    public class DistributionPercentageApiController : BaseApiController<DistributionPercentage>
    {
        private static readonly DistributionPercBuilder DistributionPerc = new DistributionPercBuilder();

        [HttpGet]
        public IEnumerable<DistributionPercentage> FetchAllData()
        {
          var disPercentageList = SessionManager.GetCurrentSession()
                .QueryOver<DistributionPercentage>().List();

            var existingProductList = disPercentageList.Select(x => x.Products)
                .Distinct().ToList();

            var productList = Enum.GetValues(typeof(ScbEnums.Products)).OfType<ScbEnums.Products>().ToList();

            var checkProductList = (from product in productList
                               where (product != ScbEnums.Products.UNKNOWN && !existingProductList.Contains(product))
                               select product)
                               .ToList();
            
            foreach (var product in checkProductList)
            {
                var disPercentage = new DistributionPercentage
                {
                    Products = product,
                    TelecallingInhouse = 0,
                    TelecallingExternal = 0,
                    FieldInhouse = 0,
                    FieldExternal = 0
                };
                disPercentageList.Add(disPercentage);
            }
            return disPercentageList;
        }

      [HttpPost]
        public HttpResponseMessage SaveProductPercentage(DistributionPercentage distribution)
        {
            DistributionPerc.Save(distribution);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}