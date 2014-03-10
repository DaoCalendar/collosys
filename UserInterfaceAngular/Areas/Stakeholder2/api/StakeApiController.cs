#region references

using System;
using System.Collections.Generic;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Shared.Attributes;

#endregion


//hierarchy calls changed
namespace ColloSys.UserInterface.Areas.Stakeholder2.api
{
    public class StakeApiController : ApiController
    {
        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<StkhHierarchy> GetAllHierarchies()
        {
            var data = HierarchyQuery.GetOnExpression(x => x.Hierarchy != "Developer");
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
        public void SaveHierarchy(StkhHierarchy stk)
        {
            HierarchyQuery.Save(stk);
        }
    }
}
