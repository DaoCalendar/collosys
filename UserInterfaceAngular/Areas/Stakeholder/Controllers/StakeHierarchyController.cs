using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Controllers;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Stakeholder.Controllers
{
    public class StakeHierarchyController : BaseController
    {
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult Index()
        {
            return View();
        }
    }
}

