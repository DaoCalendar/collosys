using System.Web.Mvc;

namespace ColloSys.UserInterface.Areas.OtherUploads
{
    public class OtherUploadsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OtherUploads";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OtherUploads_default",
                "OtherUploads/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
