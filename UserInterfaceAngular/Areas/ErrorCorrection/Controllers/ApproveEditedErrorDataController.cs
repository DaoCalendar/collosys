using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.ErrorCorrection.Controllers
{
    public class ApproveEditedErrorDataController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.FileApproval)]
        public ActionResult Index()
        {
            return View();
        }

    }
}
