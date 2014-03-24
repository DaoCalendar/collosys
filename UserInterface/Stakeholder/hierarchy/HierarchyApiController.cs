#region references

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Shared.Attributes;

#endregion


//hierarchy calls changed
namespace AngularUI.Stakeholder.hierarchy
{
    public class HierarchyApiController : ApiController
    {
        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<StkhHierarchy> GetAllHierarchies()
        {
            var data = HierarchyQuery.FilterBy(x => x.Hierarchy != "Developer");
            return data;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> GetReportingLevel()
        {
            return Enum.GetNames(typeof(ColloSysEnums.ReportingLevel));
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage SaveHierarchy(StkhHierarchy stk)
        {
            HierarchyQuery.Save(stk);
            return Request.CreateResponse(HttpStatusCode.OK, stk);
        }
    }
}
