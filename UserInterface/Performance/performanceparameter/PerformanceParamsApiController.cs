using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Performance;
using ColloSys.DataLayer.Performance.PerformanceParameter;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.PerformanceBuilder;

namespace AngularUI.Performance.performanceparameter
{
    public class PerformanceManagementApiController : BaseApiController<PerformanceParams>
    {
        private static readonly PerformanceParamBuilder Performance = new PerformanceParamBuilder();

        [HttpPost]
        public HttpResponseMessage Saveperformanace(IEnumerable<PerformanceParams> list)
        {
            Performance.Save(list);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public IEnumerable<PerformanceParams> FetchParams(ScbEnums.Products products)
        {
            var data= SessionManager.GetCurrentSession()
                                .QueryOver<PerformanceParams>()
                               .Where(x => x.Products== products)
                                .List();
            return data;
        }
    }
}