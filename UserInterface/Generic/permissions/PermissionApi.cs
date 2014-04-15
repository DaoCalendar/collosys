using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Areas.Generic.apiController;
using Newtonsoft.Json.Linq;

namespace AngularUI.Generic.permissions
{
    public class PermissionApiController : BaseApiController<StkhHierarchy>
    {

        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();


        [HttpPost]
        public void SetPermission(JObject json)
        {
            var permission = json.GetValue("json").Value<string>();
            var hierarchies = Session.QueryOver<StkhHierarchy>().List<StkhHierarchy>();
            foreach (var hierarchy in hierarchies.Where(hierarchy => hierarchy.Permissions == "[]"))
            {
                hierarchy.Permissions = permission;
                HierarchyQuery.Save(hierarchy);
            }

        }

        [HttpPost]
        public HttpResponseMessage SaveHierarchies(IEnumerable<StkhHierarchy> hierarchy)
        {
            var listOfObjects = hierarchy as StkhHierarchy[] ?? hierarchy.ToArray();
            HierarchyQuery.Save(listOfObjects);
            return Request.CreateResponse(HttpStatusCode.OK, listOfObjects);
        }

        
    }
}