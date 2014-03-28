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

        [HttpPost]
        HttpResponseMessage CheckUser(ForgotPasswordModel forgotInfo)
        {
            var users = _usersRepo.FilterBy(x => x.Username == forgotInfo.username);
            return Request.CreateResponse(HttpStatusCode.OK, users.Count == 0);
        }

        [HttpPost]
        HttpResponseMessage ResetPassword(ForgotPasswordModel forgotInfo)
        {
            var currentUser = _usersRepo.FilterBy(x => x.Username == forgotInfo.username).FirstOrDefault();
            if (currentUser == null)
            {
                return Request.CreateResponse(HttpStatusCode.PreconditionFailed, forgotInfo);
            }
            forgotInfo.email = currentUser.Email;
            forgotInfo.password = System.Web.Security.Membership.GeneratePassword(8, 0);
            currentUser.Password = PasswordUtility.EncryptText(forgotInfo.password);
            _usersRepo.Save(currentUser);
            return Request.CreateResponse(HttpStatusCode.OK, forgotInfo);
        }
    }

    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class ForgotPasswordModel
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public bool isUserValid { get; set; }
    }
}
