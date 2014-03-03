using System;
using System.Web.Mvc;

namespace ColloSys.UserInterface.Areas.Stakeholder2.Controllers
{
    public class AddStakeHolderController : Controller
    {
        //
        // GET: /Stakeholder2/AddStakeHolder/

        [HttpGet]
        public ActionResult Index()
        {
            ViewData["StakeholderId"] = Guid.Empty;
            var currentUser = HttpContext.User.Identity.Name;
            return View();
        }

        [HttpGet]
        public ActionResult EditStakeholder(Guid id)
        {
            ViewData["StakeholderId"] = id;// "5ae39d7e-1477-4f29-a4ed-a2920117f869";//"45d0eeaf-bd9c-49cb-bd4a-a29100f4855a";// Guid.NewGuid();
            return View("Index");
        }
    }
}
