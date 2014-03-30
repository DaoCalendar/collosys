#region references

using System;
using System.IO;
using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Areas.OtherUploads.Helper;
using ColloSys.UserInterface.Shared.Attributes;
using NLog;
using UserInterfaceAngular.Filters;

#endregion

namespace ColloSys.UserInterface.Areas.OtherUploads.Controllers
{
    public class RcodeUploadController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        public ActionResult Index()
        {
            return View("RcodeUpload");
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

        [HttpPost]
        [MvcTransaction]
        public JsonResult UploadRcodes(ScbEnums.Products selectedProduct)
        {
            string message;

            if (Request.Files.Count <= 0)
            {
                message = "No Rcode data for Upload.";
                _logger.Fatal("OtherUploads: Rcode : Received no files.");
                return Json(message, "text/plain");
            }

            if (Request.Files.Count > 1)
            {
                message = "Error : Received multiple files for Upload.";
                _logger.Fatal("OtherUploads: Rcode : Received multiple files.");
                return Json(message, "text/plain");
            }

            var fileBase = Request.Files[0];
            if (! RcodeUploadHelper.IsFileValid(fileBase, out message))
            {
                message = "Error : " + message;
                return Json(message, "text/plain");
            }

            try
            {
                RcodeUploadHelper.ReadRcodeExcel(selectedProduct, fileBase);
                message = "success";
            }
            catch (Exception exception)
            {
                message = exception.ToString();
            }
            return Json(message, "text/plain");
        }

    }
}
