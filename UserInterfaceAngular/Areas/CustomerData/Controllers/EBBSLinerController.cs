using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.CustomerData.Controllers
{
    public class EBBSLinerController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        public ActionResult Index()
        {
            return View();
        }

    }
}
