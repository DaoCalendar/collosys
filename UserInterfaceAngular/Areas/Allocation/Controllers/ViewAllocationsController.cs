using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Allocation.Controllers
{
    public class ViewAllocationsController : Controller
    {
        //
        // GET: /Allocation/ViewAllocations/

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Allocation)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Allocation)]
        public ActionResult ViewAllocations()
        {
            return View();
        }

    }
}
