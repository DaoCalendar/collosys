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

namespace AngularUI.Generic.Menu
{
    public class MenuApi : BaseApiController<StkhHierarchy>
    {
        private static readonly GUsersRepository GUserQueryBuilder = new GUsersRepository();

        public HttpResponseMessage GetPermission(string user)
        {
            var query = GUserQueryBuilder.ApplyRelations().Where(x => x.Username == user);
            var userInfo = GUserQueryBuilder.Execute(query).FirstOrDefault();
            return userInfo != null ? Request.CreateResponse(HttpStatusCode.OK, userInfo.Role) : null;
        }
    }
}