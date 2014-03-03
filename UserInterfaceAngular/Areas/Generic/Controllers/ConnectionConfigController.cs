using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Generic.Controllers
{
    public class ConnectionConfigController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult Index()
        {
            return View();
        }

    }
}
