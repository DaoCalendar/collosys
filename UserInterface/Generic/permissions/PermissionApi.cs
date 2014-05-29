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
using ColloSys.QueryBuilder.Test.GenerateDb;
using ColloSys.QueryBuilder.TransAttributes;
using ColloSys.UserInterface.Areas.Generic.apiController;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AngularUI.Generic.permissions
{
    public class PermissionApiController : BaseApiController<GPermission>
    {

        private static readonly GUsersRepository GUserQueryBuilder = new GUsersRepository();
        private static readonly GPermissionBuilder PermQuery = new GPermissionBuilder();
        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();

        [HttpGet]
        public HttpResponseMessage GetStakeHierarchy()
        {
            var hierData = Session.QueryOver<StkhHierarchy>().List();
            return Request.CreateResponse(HttpStatusCode.OK, hierData);
        }

        public void parseList(GPermission permissions)
        {
            if (permissions.Childrens == null || permissions.Childrens.Count == 0) return;
            foreach (var gPermission in permissions.Childrens)
            {
                parseList(gPermission);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPermission(Guid id)
        {
            var permData = Session.Query<GPermission>()
                                     .Where(x => x.Role.Id == id && x.Parent == null)
                                     .Fetch(x => x.Role)
                                     .ToList();

            switch (permData.Count)
            {
                case 0:
                    {
                        var hierarchy = Session.QueryOver<StkhHierarchy>().Where(x => x.Id == id).SingleOrDefault();
                        GPermission data = PermissionManager.CreateDevPermissions(hierarchy);
                        data = PermissionManager.SetAccess(data, false);
                        parseList(data);
                        return Request.CreateResponse(HttpStatusCode.OK, data);
                    }
                default:
                    parseList(permData[0]);
                    return Request.CreateResponse(HttpStatusCode.OK, permData[0]);
            }



            //GPermission parent = null;
            //GPermission children = null;
            //GPermission grandChildren = null;

            //var permData = Session.QueryOver<GPermission>()
            //        .Where(x => x.Role.Id == id)
            //        .And(x => x.Parent == null)
            //        .Fetch(x => x.Role).Eager
            //        .Fetch(x => x.Childrens).Eager
            //        .Fetch(x=>x.Parent).Eager
            //        .List<GPermission>();

            //var permData = Session.QueryOver<GPermission>(() => parent)
            //                      .Fetch(x => x.Childrens).Eager
            //                      .Fetch(x => x.Role).Eager
            //                      .JoinAlias(() => parent.Childrens, () => children, JoinType.InnerJoin)
            //                      .JoinAlias(() => children.Childrens, () => grandChildren, JoinType.InnerJoin)
            //                      .Where(() => parent.Role.Id == id)
            //                      .And(() => parent.Parent == null)
            //                      .TransformUsing(Transformers.DistinctRootEntity)
            //                      .List();

            //var setting = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;
            //setting.PreserveReferencesHandling = PreserveReferencesHandling.All;
            //var data = JsonConvert.SerializeObject(permData[0], setting);
            //setting.PreserveReferencesHandling = PreserveReferencesHandling.None;
            //if (permData == null || permData.Count == 0)
            //{
            //    var hierarchy = Session.QueryOver<StkhHierarchy>()
            //        .Where(x => x.Designation == "Developer")
            //        .And(x => x.Hierarchy == "Developer")
            //        .SingleOrDefault();

            //    permData = Session.QueryOver<GPermission>()
            //        .Where(x => x.Role.Id == hierarchy.Id)
            //        .And(x => x.Parent == null)
            //        .Fetch(x => x.Role).Eager
            //        .Fetch(x => x.Parent).Eager
            //        .TransformUsing(Transformers.DistinctRootEntity)
            //        .List<GPermission>();
            //}
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