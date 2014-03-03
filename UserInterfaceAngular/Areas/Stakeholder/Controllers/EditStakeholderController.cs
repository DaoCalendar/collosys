using System.Web.Mvc;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Stakeholder.Controllers
{
    public class EditStakeholderController : Controller
    {
        [UserActivity(Activity = ColloSysEnums.Activities.Stakeholder)]
        public ActionResult Index()
        {
            return View();
        }

        [UserActivity(Activity = ColloSysEnums.Activities.Stakeholder)]
        public ActionResult Approve()
        {
            return View();
        }
    }
}
