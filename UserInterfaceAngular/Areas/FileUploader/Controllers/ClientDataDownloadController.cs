using System.IO;
using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.FileUploader.Controllers
{
    public class ClientDataDownloadController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        public ActionResult Index()
        {
            return View("ClientDataDownload");
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        public FileResult Download(string fullfilename)
        {
            fullfilename = fullfilename.Substring(2).Replace("\\\\", "\\").Replace("\"'", "");
            var fileinfo = new FileInfo(fullfilename);

            if (!fileinfo.Exists)
            {
                throw new FileNotFoundException(fileinfo.Name);
            }

            return File(fileinfo.FullName, System.Net.Mime.MediaTypeNames.Application.Zip, fileinfo.Name);
        }
    }
}
