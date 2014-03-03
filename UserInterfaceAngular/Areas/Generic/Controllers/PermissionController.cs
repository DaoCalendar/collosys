using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Controllers;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Generic.Controllers
{
    public class PermissionController : BaseController
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PermissionScreen()
        {
            return View();
        }
    }
}


//[HttpGet]
//public JsonResult GetPermissions()
//{
//    return Json(GPermissionServices.GetAllData());
//}

//[HttpPost]
//public void Post(IEnumerable<JObject> gPermissions)
//{
//    var permissions = gPermissions.Select(c => c.ToObject<GPermission>()).ToList();

//    GPermissionServices.SaveOrUpdate(permissions);
//}

//[HttpGet]
//public JsonResult GetActivity()
//{
//    return Json(new string[] { "Allocation Sub Policy", "Allocation Policy", "Allocation Modification", "Allocation Approval" });

//    //return Json(activity.Select(c => JObject.FromObject(new { Activity = c })).ToList();
//}

//[HttpGet]
//public JsonResult GetHierarchy()
//{
//    return Json(StakeHierarchyService.GetAll());
//}
