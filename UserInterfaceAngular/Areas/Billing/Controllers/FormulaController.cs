using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Controllers;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Billing.Controllers
{
    public class FormulaController : BaseController
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Billing)]
        public ActionResult Index()
        {
            return View();
        }

    }
}
