using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Allocation.Controllers
{
    public class AllocationPolicyController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Allocation)]
        public ActionResult Index()
        {
            return View();
        }
    }
}
