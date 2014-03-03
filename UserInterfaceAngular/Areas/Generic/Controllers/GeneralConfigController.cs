using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Areas.Generic.Models;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Generic.Controllers
{
    public class GeneralConfigController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult EncryptData()
        {
            const string sectionName = "connectionStrings";
            Cryptography.EncryptConnString(sectionName);
            return View("Index");
        }

        [HttpPost]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult DecryptData()
        {
            const string sectionName = "connectionStrings";
            Cryptography.DecryptConnString(sectionName);
            return View("Index");
        }

        //[HttpPost]
        //[UserActivity(Activity = EnumHelper.Activities.Development)]
        //public ActionResult GetAllConnectionStrings()
        //{
        //    var data = ConnectionStringMgr.GetAllConnectionStrings();
        //    return View("Index");
        //}
    }
}
