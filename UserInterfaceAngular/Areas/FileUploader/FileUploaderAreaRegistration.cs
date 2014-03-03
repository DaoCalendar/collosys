using System.Web.Mvc;

namespace ColloSys.UserInterface.Areas.FileUploader
{
    public class FileUploaderAreaRegistration : AreaRegistration
    {
        public override string AreaName { get { return "FileUploader"; } }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "FileUploader_default",
                "FileUploader/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}


//context.MapRoute(
//    "collosys_FileUploader_default",
//    "collosys/FileUploader/{controller}/{action}/{id}",
//    new { action = "Index", id = UrlParameter.Optional }
//);


