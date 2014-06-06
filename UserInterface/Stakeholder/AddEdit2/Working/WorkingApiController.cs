using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Stakeholder;
using NHibernate.Linq;

namespace AngularUI.Stakeholder.AddEdit2.Working
{
    public class WorkingApiController : BaseApiController<StkhWorking>
    {
        [HttpGet]
        public HttpResponseMessage GetStakeWorkingData(Guid stakeholderId)
        {
            var singleOrDefault = Session.Query<Stakeholders>().Where(x => x.Id == stakeholderId)
                .Fetch(x => x.Hierarchy).SingleOrDefault();
            if (singleOrDefault == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            var data = new
                {
                    HierarchyData = singleOrDefault.Hierarchy,
                    ReportsToList = Session.QueryOver<Stakeholders>()
                                           .Where(x => x.Id != stakeholderId).List()
                };

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}