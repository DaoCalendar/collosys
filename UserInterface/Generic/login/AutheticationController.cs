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
    public class AutheticationController : ApiController
    {
        private readonly IRepository<Users> _usersRepo = new GUsersRepository();

        [HttpPost]
        public HttpResponseMessage AutheticateUser(string login, string password)
        {
            var users = _usersRepo.FilterBy(x => x.Username == login);
            var autheticated = false;
            if (users.Count != 1) return Request.CreateResponse(HttpStatusCode.OK, false);
            if (PasswordUtility.DoMatch(password, users.First().Password))
            {
                autheticated = true;
            }

            return Request.CreateResponse(HttpStatusCode.OK, autheticated);
        }
    }
}
