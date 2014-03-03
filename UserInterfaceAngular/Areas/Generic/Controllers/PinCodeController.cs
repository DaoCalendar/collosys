using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Controllers;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Generic.Controllers
{
    public class PinCodeController : BaseController
    {
        //
        // GET: /Generic/PinCode/
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Allocation)]
        public ActionResult Index()
        {
            return View();
        }

    }
}
