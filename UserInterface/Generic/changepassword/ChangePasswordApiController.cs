using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.Shared.Encryption;
using ColloSys.Shared.SharedUtils;
using Glimpse.AspNet.Tab;

namespace AngularUI.Generic.changepassword
{
    public class ChangePasswordApiController : BaseApiController<GUsers>
    {
        private readonly IRepository<GUsers> _usersRepo = new GUsersRepository();

        [HttpPost]
        public HttpResponseMessage ChangedPassword(ChangePasswordModel changedpassword)
        {
            var currentpassword = PasswordUtility.EncryptText(changedpassword.Currentpassword);

            var newpassword = PasswordUtility.EncryptText(changedpassword.Newpassword);

            var confirmpassword = PasswordUtility.EncryptText(changedpassword.Confirmpassword);

            var data = _usersRepo.FilterBy(x => x.Password == currentpassword)
                .FirstOrDefault();
            if (data != null)
            {
                data.Password = newpassword;
                _usersRepo.Save(data);
            }
            else
            {
                throw new Exception("User does not exist or passwords do not match!!!");
            }

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

    }
}
