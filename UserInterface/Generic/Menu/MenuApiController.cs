using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.GenericBuilder;

namespace AngularUI.Generic.Menu
{
    public class MenuApiController : BaseApiController<StkhHierarchy>
    {
        private static readonly GUsersRepository GUserQueryBuilder = new GUsersRepository();

        [HttpGet]
        public HttpResponseMessage GetPermission(string user)
        {
            var query = GUserQueryBuilder.ApplyRelations().Where(x => x.Username == user);
            var userData = GUserQueryBuilder.Execute(query).FirstOrDefault();

            var root = Session.QueryOver<GPermission>()
                    .Where(x => x.Role.Id == userData.Role.Id)
                    .And(x => x.Parent == null)
                    .List<GPermission>();

            var menu = new MenuManager();
            var ma = menu.CreateMenu();

            ma = (root == null || root.Count == 0) ? MenuManager.DefaultMenu(ma) : MenuManager.CreateAutherizedMenu(root[0], ma);
            return Request.CreateResponse(HttpStatusCode.OK, ma);


        }
    }
}