using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.QueryBuilder.GenericBuilder;

namespace AngularUI.Generic.taxmaster
{
    public class TaxMasterApiController : ApiController
    {
        private static readonly GPincodeBuilder GPincodeBuilder = new GPincodeBuilder();

        [HttpGet]
        public HttpResponseMessage StateList()
        {
            var data = GPincodeBuilder.StateList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}
