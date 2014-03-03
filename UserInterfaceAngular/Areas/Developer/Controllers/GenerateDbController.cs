#region references

using System.Web.Mvc;
using System.Web.Security;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Areas.Developer.Models;
using ColloSys.UserInterface.Shared.Attributes;
using UserInterfaceAngular.Filters;

#endregion

namespace ColloSys.UserInterface.Areas.Developer.Controllers
{
    public class GenerateDbController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult GenerateDB()
        {
            return View("GenerateDB");
        }

        [HttpGet]
        [MvcTransaction]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult SignOut()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                Session.Abandon();
            }

            return RedirectToAction("Login", "Account", new {area = string.Empty});
        }

    }
}
