using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ColloSys.UserInterface.Areas.Stakeholder2.Controllers
{
    public class StakeholerViewController : Controller
    {
        //
        // GET: /Stakeholder2/StakeholerView/

        public ActionResult ViewStakeholder()
        {
            return View();
        }

        public ActionResult WorkingManage()
        {
            return View();
        }
    }
}
