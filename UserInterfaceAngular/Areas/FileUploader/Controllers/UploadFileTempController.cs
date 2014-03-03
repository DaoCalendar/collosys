using System;
using System.IO;
using System.Web.Mvc;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploadService.Excel2DT;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.FileUploader.Controllers
{
    public class UploadFileTempController : Controller
    {
        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult Upload()
        {
            var files = Request.Files.Count;
            if (files == 0)
                return View("Index");

            var file = Request.Files["uploadFile"];
            var dir = System.IO.Path.GetTempPath();

            if (file != null) file.SaveAs(dir + file.FileName);

            return View("Index");
        }

       
    }
}
