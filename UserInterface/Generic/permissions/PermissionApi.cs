using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using NHibernate.Transform;
using Newtonsoft.Json.Linq;

namespace AngularUI.Generic.permissions
{
    public class PermissionApiController : BaseApiController<GPermission>
    {

        private static readonly GPermissionBuilder PermQuery = new GPermissionBuilder();
        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();

        [HttpGet]
        public HttpResponseMessage GetStakeHierarchy()
        {
            var hierData = Session.QueryOver<StkhHierarchy>().List();
            return Request.CreateResponse(HttpStatusCode.OK, hierData);
        }

        [HttpGet]
        public HttpResponseMessage GetPermission(Guid id)
        {
            var permData = Session.QueryOver<GPermission>()
                    .Where(x => x.Role.Id == id)
                    .And(x => x.Parent == null)
                    .Fetch(x => x.Role).Eager
                    .Fetch(x => x.Parent).Eager
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .List<GPermission>();

            if (permData != null && permData.Count != 0) 
                return Request.CreateResponse(HttpStatusCode.OK, permData);

            var hierarchy = Session.QueryOver<StkhHierarchy>()
                .Where(x => x.Designation == "Developer")
                .And(x => x.Hierarchy == "Developer")
                .SingleOrDefault();

            permData = Session.QueryOver<GPermission>()
                .Where(x => x.Role.Id == hierarchy.Id)
                .And(x => x.Parent == null)
                .Fetch(x => x.Role).Eager
                .Fetch(x => x.Parent).Eager
                .TransformUsing(Transformers.DistinctRootEntity)
                .List<GPermission>();

            return Request.CreateResponse(HttpStatusCode.OK, permData);
        }


        [HttpPost]
        public HttpResponseMessage SavePerm(GPermission data)
        {
            foreach (var activity in data.Childrens)
            {
                var subActivityParent = activity;
                activity.Parent = data;
                foreach (var subActivity in activity.Childrens)
                {
                    var childParent = subActivity;
                    subActivity.Parent = subActivityParent;
                    foreach (var child in subActivity.Childrens)
                    {
                        child.Parent = childParent;
                    }
                }
            }

            PermQuery.Save(data);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

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