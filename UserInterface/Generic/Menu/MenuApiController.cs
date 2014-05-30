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
using ColloSys.QueryBuilder.Test.GenerateDb;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace AngularUI.Generic.Menu
{
    public class MenuApiController : BaseApiController<StkhHierarchy>
    {
        private static readonly GUsersRepository GUserQueryBuilder = new GUsersRepository();
        private static readonly GPermissionBuilder PermQuery = new GPermissionBuilder();

        //public HttpResponseMessage GetPermission(string user)
        //{
        //    var query = GUserQueryBuilder.ApplyRelations().Where(x => x.Username == user);
        //    var userInfo = GUserQueryBuilder.Execute(query).FirstOrDefault();
        //    return userInfo != null ? Request.CreateResponse(HttpStatusCode.OK, userInfo.Role) : null;
        //}

        public HttpResponseMessage GetPermission(string user)
        {
            var query = GUserQueryBuilder.ApplyRelations().Where(x => x.Username == user);
            var userData = GUserQueryBuilder.Execute(query).Single();
            var permission = PermissionManager.GetPermission(userData.Role);

            var menuMgr = new MenuManager();
            var menu = menuMgr.CreateMenu();

            MenuManager.CreateAutherizedMenu(permission, menu);

            return Request.CreateResponse(HttpStatusCode.OK, menu);


        }
    }
}
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