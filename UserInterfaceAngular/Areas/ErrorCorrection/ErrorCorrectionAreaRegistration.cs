using System.Web.Mvc;

namespace ColloSys.UserInterface.Areas.ErrorCorrection
{
    public class ErrorCorrectionAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ErrorCorrection";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ErrorCorrection_default",
                "ErrorCorrection/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
