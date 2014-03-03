using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Developer.Controllers
{
    public class DatabaseTablesController : Controller
    {
        //
        // GET: /Developer/DatabaseTables/

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
        public ActionResult DownLoadDatabaseTables()
        {
            return View();
        }

        [HttpGet]
        [UserActivity(Activity = ColloSysEnums.Activities.Development)]
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
