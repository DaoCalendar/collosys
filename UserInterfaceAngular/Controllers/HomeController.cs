using System.Linq;
using System.Web.Mvc;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Services.Shared;
using ColloSys.UserInterface.Shared;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Controllers
{
    public class HomeController : Controller
    {
        [UserActivity]
        public ActionResult Index()
        {
            //var id = System.Web.HttpContext.Current.User.Identity.Name;
            //var session = SessionManager.GetCurrentSession();
            //var permission = AuthService.GetPremissionsForCurrentUser();
            //var data = new PendingApprovalData
            //    {
            //        stakeholder = session.QueryOver<Stakeholders>().Where(x => x.ApprovedBy == id
            //            && x.Status == ColloSysEnums.ApproveStatus.Submitted).List().Count(),
            //        working = session.QueryOver<StkhWorking>().Where(x => x.ApprovedBy == id
            //            && x.Status == ColloSysEnums.ApproveStatus.Submitted).List().Count(),
            //        payment = session.QueryOver<StkhPayment>().Where(x => x.ApprovedBy == id
            //            && x.Status == ColloSysEnums.ApproveStatus.Submitted).List().Count(),
            //        allocation = session.QueryOver<AllocRelation>().Where(x => x.ApprovedBy == id
            //            && x.Status == ColloSysEnums.ApproveStatus.Submitted).List().Count()
            //    };
            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }
    }
}
