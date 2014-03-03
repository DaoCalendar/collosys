using System.Web.Mvc;

namespace ColloSys.UserInterface.Areas.Reporting
{
    public class ReportingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Reporting";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.MapRoute(
            //    "collosys_Reporting_default",
            //    "collosys/Reporting/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
                "Reporting_default",
                "Reporting/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
