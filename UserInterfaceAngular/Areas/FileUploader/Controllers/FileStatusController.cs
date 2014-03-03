#region references

using System.IO;
using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Controllers;
using UserInterfaceAngular.Filters;

#endregion

namespace ColloSys.UserInterface.Areas.FileUploader.Controllers
{
    public class FileStatusController : BaseController
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        public FileResult Download(string fullfilename)
        {
            fullfilename = fullfilename.Replace("\\\\", "\\").Replace("\"'", "");
           if (!char.IsLetter(fullfilename[0]))
           {
               fullfilename = fullfilename.Substring(2);
           }

            var fileinfo = new FileInfo(fullfilename);
            if (!fileinfo.Exists)
            {
                throw new FileNotFoundException(fileinfo.Name);
            }

            return File(fileinfo.FullName, System.Net.Mime.MediaTypeNames.Application.Zip, fileinfo.Name);
        }
    }
}

