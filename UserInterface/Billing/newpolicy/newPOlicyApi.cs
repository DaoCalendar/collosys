using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.BillingBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using Glimpse.AspNet.Tab;
using NHibernate.Mapping;

namespace AngularUI.Billing.newpolicy
{
    public class NewpolicyApiController : BaseApiController<BillingPolicy>
    {
        [HttpGet]
        public HttpResponseMessage GetStakeholerOrHier(string policyfor)
        {
            if (policyfor == "Stakeholder")
            {
                 var data = Session.QueryOver<StkhHierarchy>().Select(x=>x.Designation)
                     .List<string>()
                     .Distinct();
                 return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            else
            {
                var data = Session.QueryOver<StkhHierarchy>().Select(x => x.Hierarchy)
                .List<string>()
                .Distinct();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
           
        }
    }
}