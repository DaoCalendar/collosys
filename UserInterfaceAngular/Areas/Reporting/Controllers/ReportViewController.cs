using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Reporting.Controllers
{
    public class ReportViewController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Reporting)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [UserActivity(Activity = ColloSysEnums.Activities.Reporting)]
        public ActionResult Create()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [UserActivity(Activity = ColloSysEnums.Activities.Reporting)]
        public ActionResult Edit()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [UserActivity(Activity = ColloSysEnums.Activities.Reporting)]
        public ActionResult Delete()
        {
            return RedirectToAction("Index");
        }
    }
}
