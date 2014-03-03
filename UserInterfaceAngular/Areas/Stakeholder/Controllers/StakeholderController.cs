using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Stakeholder.Controllers
{
    public class StakeholderController : Controller
    {
        [UserActivity(Activity = ColloSysEnums.Activities.Stakeholder)]
        public ActionResult AddStakeholder()
        {
            return View();
        }
    }
}
