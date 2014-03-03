using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.FileUploader.Controllers
{
    public class FileDetailsController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult Index()
        {
            return View();
        }

    }
}
