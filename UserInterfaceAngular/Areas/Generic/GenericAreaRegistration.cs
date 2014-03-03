using System.Web.Mvc;

namespace ColloSys.UserInterface.Areas.Generic
{
    public class GenericAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Generic";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Generic_default",
                "Generic/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
