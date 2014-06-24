using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Generic;
using ColloSys.QueryBuilder.GenericBuilder;

namespace AngularUI.Generic.performancemanagement
{
    public class PerformanceManagementApiController : BaseApiController<PerformanceManagement>
    {
        private static readonly PerformanceManagementBuilder Performance = new PerformanceManagementBuilder();


        protected override PerformanceManagement BasePost(PerformanceManagement obj)
        {
            foreach (var performanceParam in obj.PerformanceParamses)
            {
                performanceParam.PerformanceManagement = obj;
            }
            Performance.Save(obj);
            return obj;
        }
        [HttpPost]
        public HttpResponseMessage Saveperformanace(PerformanceManagement objManagement)
        {
            var data = BasePost(objManagement);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}