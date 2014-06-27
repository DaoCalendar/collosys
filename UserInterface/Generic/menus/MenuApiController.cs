using System.Linq;
using System.Net;
using System.Net.Http;
using AngularUI.Generic.permissions;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.GenericBuilder;

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
            var permission = PermissionManager.GetPermission(userData.Role,user);

            var menuMgr = new MenuManager();
            var menu = menuMgr.CreateMenu();

            MenuManager.CreateAutherizedMenu(permission, menu);

            var menuPermData =
               new
               {
                   menus = menu,
                   permissions = permission
               };

            return Request.CreateResponse(HttpStatusCode.OK, menuPermData);


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