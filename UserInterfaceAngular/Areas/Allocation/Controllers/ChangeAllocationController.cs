using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Allocation.Controllers
{
    public class ChangeAllocationController : Controller
    {
        //
        // GET: /Allocation/ChangeAllocation/

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Allocation)]
        public ActionResult Index()
        {
            return View();
        }

    }
}
