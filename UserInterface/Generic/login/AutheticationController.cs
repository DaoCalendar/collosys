#region references

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.Shared.Encryption;
using EMailServices;

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
        public HttpResponseMessage CheckUser(ForgotPasswordModel forgotInfo)
        {
            var users = _usersRepo.FilterBy(x => x.Username == forgotInfo.username);
            return Request.CreateResponse(HttpStatusCode.OK, users.Count != 0);
        }

        [HttpPost]
        public HttpResponseMessage ResetPassword(ForgotPasswordModel forgotInfo)
        {
            var currentUser = _usersRepo.FilterBy(x => x.Username == forgotInfo.username).FirstOrDefault();
            if (currentUser == null)
            {
                return Request.CreateResponse(HttpStatusCode.PreconditionFailed, forgotInfo);
            }
            forgotInfo.email = currentUser.Email;
            //forgotInfo.password = System.Web.Security.Membership.GeneratePassword(8, 0);
            forgotInfo.password = Guid.NewGuid()
                .ToString("d").Replace("-", "")
                .Replace("1", "").Replace("l", "")
                .Replace("0", "").Replace("o", "")
                .Substring(1, 8);
            currentUser.Password = PasswordUtility.EncryptText(forgotInfo.password);
            _usersRepo.Save(currentUser);
            ResetPasswordEmail.Send(forgotInfo.email, forgotInfo.password);
            return Request.CreateResponse(HttpStatusCode.OK, forgotInfo);
        }
    }
}
