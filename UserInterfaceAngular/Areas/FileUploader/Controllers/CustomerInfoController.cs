using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.FileUploader.Controllers
{
    public class CustomerInfoController : Controller
    {
        //
        // GET: /FileUploader/CustomerInfo/

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Allocation)]

        public ActionResult Index()
        {
            return View();
        }

    }
}
