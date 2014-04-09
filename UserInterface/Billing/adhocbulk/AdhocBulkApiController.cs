using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.QueryBuilder.GenericBuilder;

namespace AngularUI.Billing.adhocbulk
{
    public class AdhocBulkApiController : ApiController
    {
        private static readonly ProductConfigBuilder ProductConfigBuilder = new ProductConfigBuilder();

        [HttpGet]
        public HttpResponseMessage GetProducts()
        {
            var data = ProductConfigBuilder.GetProducts();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}
