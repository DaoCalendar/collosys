using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularUI.Generic.changepassword
{
    public class ChangePasswordModel
    {
        public string Currentpassword { get; set; }

        public string Newpassword { get; set; }

        public string Confirmpassword { get; set; }
    }
}