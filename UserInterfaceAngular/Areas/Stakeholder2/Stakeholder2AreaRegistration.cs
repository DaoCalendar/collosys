using System.Web.Mvc;

namespace ColloSys.UserInterface.Areas.Stakeholder2
{
    public class Stakeholder2AreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Stakeholder2";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Stakeholder2_default",
                "Stakeholder2/{controller}/{action}/{id}",
                new {controller = "AddStakeHolder", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}
