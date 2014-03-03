using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ColloSys.DataLayer.Enumerations;
using NLog;
using UserInterfaceAngular.Filters;

namespace ColloSys.UserInterface.Areas.Developer.Controllers
{
    public class LogDownloadController : Controller
    {
        //
        // GET: /Developer/LogDownload1/

        public ActionResult Index()
        {
            return View();
        }

        //[System.Web.Mvc.HttpGet]
        ////[UserActivity(Activity = ColloSysEnums.Activities.FileUploader)]
        //public FileResult DownloadFile(string fullfilename)
        //{
        //    fullfilename = fullfilename.Replace("\\\\", "\\").Replace("\"'", "");
        //    //if (!char.IsLetter(fullfilename[0]))
        //    //{
        //    //    fullfilename = fullfilename.Substring(2);
        //    //}

        //    var fileinfo = new FileInfo(fullfilename);
        //    if (!fileinfo.Exists)
        //    {
        //        throw new FileNotFoundException(fileinfo.Name);
        //    }

        //    return System.IO.File(fileinfo.FullName, System.Net.Mime.MediaTypeNames.Application.Zip, fileinfo.Name);
        //}


        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        public FileResult Download(string filePathNName)
        {
            if (!System.IO.File.Exists(filePathNName))
            {
                _logger.Error("LogDownload : file Does Not Exist" + filePathNName);
                throw new InvalidDataException("File Does Not Exist. " + filePathNName);
            }

            var fileinfo = new FileInfo(filePathNName);
            try
            {
                var destFile = Path.GetTempPath() + DateTime.Now.ToString("HHmmssfff") + "_" + fileinfo.Name;
                var newfile = fileinfo.CopyTo(destFile, true);
                _logger.Info(string.Format("LogDownload : download copy of log {0} as {1}", fileinfo.FullName, newfile.FullName));
                return File(newfile.FullName, System.Net.Mime.MediaTypeNames.Text.Plain, newfile.Name);
            }
            catch (IOException exception)
            {
                _logger.InfoException("LogDownload : Could not access file " + fileinfo.FullName, exception);
                FileResult fileResult = null;
                var directory = fileinfo.DirectoryName;
                if (string.IsNullOrEmpty(directory))
                {
                    directory = Server.MapPath("~");
                }

                _logger.Info(string.Format("LogDownload : added watch for directory {0} & file {1} ", directory, fileinfo.Name));
                var watcher = new FileSystemWatcher(directory, fileinfo.Name);
                watcher.Changed += (sender, args) => fileResult = ReadFile(fileinfo);
                while (true)
                {
                    watcher.WaitForChanged(WatcherChangeTypes.Changed);
                    _logger.Info(string.Format("LogDownload : returning file {0} ", fileinfo.Name));
                    return fileResult;
                }
            }
        }

        private FileResult ReadFile(FileInfo fileInfo)
        {
            _logger.Info(string.Format("LogDownload : reading file {0} ", fileInfo.FullName));
            return File(fileInfo.FullName, System.Net.Mime.MediaTypeNames.Text.Plain, fileInfo.Name);
        }

    }
}
