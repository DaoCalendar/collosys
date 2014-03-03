using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Billing.Controllers
{
    public class MatrixController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Billing)]
        public ActionResult Index()
        {
            return View();
        }

    }
}
