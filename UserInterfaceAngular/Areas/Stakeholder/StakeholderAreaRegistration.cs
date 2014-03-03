using System.Web.Mvc;

namespace ColloSys.UserInterface.Areas.Stakeholder
{
    public class StakeholderAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Stakeholder";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.MapRoute(
            //    "collosys_Stakeholder_default",
            //    "collosys/Stakeholder/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
                "Stakeholder_default",
                "Stakeholder/{controller}/{action}/{id}",
                new {action = "Index", id = UrlParameter.Optional }
            );
        }


    }
}
