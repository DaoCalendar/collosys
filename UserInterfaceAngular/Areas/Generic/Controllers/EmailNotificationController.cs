using System;
using System.Web.Mvc;
using System.Web.Security;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Areas.Generic.Models;
using NLog;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Generic.Controllers
{
    public class EmailController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult SendEmail()
        {
            try
            {
                var user = HttpContext.User.Identity.Name;
                var userInfo = Membership.GetUser(user);
                TempData["status"] = EmailNotification.SendNotificationEmail(userInfo.Email, userInfo.UserName);
            }
            catch (Exception e)
            {
                _logger.Fatal(e.Message);
            }
            return RedirectToAction("Index");

        }

    }
}
