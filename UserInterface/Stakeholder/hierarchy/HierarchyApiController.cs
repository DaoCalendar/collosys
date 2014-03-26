﻿#region references

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.StakeholderBuilder;

#endregion


//hierarchy calls changed
namespace AngularUI.Stakeholder.hierarchy
{
    public class HierarchyApiController : ApiController
    {
        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();

        [HttpGet]
        
        public IEnumerable<StkhHierarchy> GetAllHierarchies()
        {
            var data = HierarchyQuery.FilterBy(x => x.Hierarchy != "Developer");
            return data;
        }

        [HttpGet]
        
        public IEnumerable<string> GetReportingLevel()
        {
            return Enum.GetNames(typeof(ColloSysEnums.ReportingLevel));
        }

        [HttpPost]
        
        public HttpResponseMessage SaveHierarchy(StkhHierarchy stk)
        {
            HierarchyQuery.Save(stk);
            return Request.CreateResponse(HttpStatusCode.OK, stk);
        }
    }
}
