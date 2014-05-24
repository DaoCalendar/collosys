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
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Newtonsoft.Json.Linq;

namespace AngularUI.Generic.permissions
{
    public class PermissionApiController : BaseApiController<GPermission>
    {

        private static readonly GUsersRepository GUserQueryBuilder = new GUsersRepository();
        private static readonly GPermissionBuilder PermQuery = new GPermissionBuilder();
        private static readonly HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();



        public HttpResponseMessage GetPermission(Guid id)
        {

            GPermission parent = null;
            GPermission childern = null;
            GPermission grandChildren = null;

            var permData = Session.QueryOver<GPermission>(() => parent)
                                  .Fetch(x => x.Childrens).Eager
                                  .Fetch(x => x.Role).Eager
                                  .JoinAlias(() => parent.Childrens, () => childern, JoinType.InnerJoin)
                                  .JoinAlias(() => childern.Childrens, () => grandChildren, JoinType.InnerJoin)
                                  .Where(() => parent.Role.Id == id)
                                  .And(() => parent.Parent == null)
                                  .TransformUsing(Transformers.DistinctRootEntity)
                                  .List();

            if (permData == null)
            {
                permData = Session.QueryOver<GPermission>(() => parent)
                                  .Fetch(x => x.Childrens).Eager
                                  .Fetch(x => x.Role).Eager
                                  .JoinAlias(() => parent.Childrens, () => childern, JoinType.InnerJoin)
                                  .JoinAlias(() => childern.Childrens, () => grandChildren, JoinType.InnerJoin)
                                  .Where(() => parent.Role.Id == Guid.Parse("2cdaf45b-52d5-4181-b9f8-a23201155b3c"))
                                  .And(() => parent.Parent == null)
                                  .TransformUsing(Transformers.DistinctRootEntity)
                                  .List();
            }

            return Request.CreateResponse(HttpStatusCode.OK, permData);

            //var permData = Session.QueryOver<GPermission>()
            //        .Where(x => x.Role.Id == userData.Role.Id)
            //        .And(x => x.Parent == null)
            //        .Fetch(x=>x.Childrens).Eager
            //        .Fetch(x=>x.Role).Eager
            //        .TransformUsing(Transformers.DistinctRootEntity)
            //        .List<GPermission>();


            //var permData = PermQuery.Execute(permQuery);

            //var root = session.QueryOver<GPermission>()
            //    .Where(x => x.Role.Id == hierarchy.Id)
            //    .And(x => x.Parent == null)
            //    //.TransformUsing(Transformers.DistinctRootEntity)
            //    .List<GPermission>();
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