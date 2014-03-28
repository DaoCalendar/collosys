#region references

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.Shared.Encryption;

#endregion

namespace AngularUI.Generic.login
{
    public class AutheticationApiController : ApiController
    {
        private readonly IRepository<Users> _usersRepo = new GUsersRepository();

        [HttpPost]
        
        public HttpResponseMessage AutheticateUser(LoginModel loginInfo)
        {
            var users = _usersRepo.FilterBy(x => x.Username == loginInfo.username);
            var autheticated = false;
            if (users.Count != 1) return Request.CreateResponse(HttpStatusCode.OK, false);
            if (PasswordUtility.DoMatch(loginInfo.password, users.First().Password))
            {
                autheticated = true;
            }

            return Request.CreateResponse(HttpStatusCode.OK, autheticated);
        }
    }

    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
