#region references

using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Areas.OtherUploads.Helper;
using ColloSys.UserInterface.Shared.Attributes;
using NLog;
using UserInterfaceAngular.Filters;

#endregion

namespace ColloSys.UserInterface.Areas.OtherUploads.Controllers
{
    public class PincodeUploadController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        public ActionResult Index()
        {
            return View("PincodeUpload");
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
        public JsonResult UploadPincodes(ScbEnums.Products selectedProduct)
        {
            string message;

            if (Request.Files.Count <= 0)
            {
                message = "No Pincode data for Upload.";
                _logger.Fatal("OtherUploads: Pincode : Received no files.");
                return Json(message, "text/plain");
            }

            if (Request.Files.Count > 1)
            {
                message = "Error : Received multiple files for Upload.";
                _logger.Fatal("OtherUploads: Pincode : Received multiple files.");
                return Json(message, "text/plain");
            }

            var fileBase = Request.Files[0];
            if (! PincodeUploadHelper.IsFileValid(fileBase, out message))
            {
                message = "Error : " + message;
                return Json(message, "text/plain");
            }

            try
            {
                PincodeUploadHelper.ReadPincodeExcel(selectedProduct, fileBase);
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
