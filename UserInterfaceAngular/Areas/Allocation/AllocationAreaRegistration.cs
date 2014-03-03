using System.Web.Mvc;

namespace ColloSys.UserInterface.Areas.Allocation
{
    public class AllocationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Allocation";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.MapRoute(
            //    "collosys_Allocation_default",
            //    "collosys/Allocation/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
                "Allocation_default",
                "Allocation/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
