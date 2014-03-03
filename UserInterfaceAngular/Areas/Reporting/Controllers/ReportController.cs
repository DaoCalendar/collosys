using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Reporting.Controllers
{
    public class ReportController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Reporting)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        public ActionResult ClientDataReport()
        {
            return View();
        }

   
    }
}
