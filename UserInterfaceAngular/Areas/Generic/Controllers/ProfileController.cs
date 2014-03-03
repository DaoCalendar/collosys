using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Controllers;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Generic.Controllers
{

    public class ProfileController : BaseController
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.All)]
        public ActionResult UserProfile()
        {
            return View();
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.All)]
        public ActionResult ModalTest()
        {
            return View();
        }

    }
}
