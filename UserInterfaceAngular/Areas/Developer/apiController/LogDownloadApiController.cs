#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.Shared.ConfigSectionReader;
using ColloSys.UserInterface.Areas.Developer.Models;
using ColloSys.UserInterface.Areas.Developer.ViewModels;

#endregion


namespace ColloSys.UserInterface.Areas.Developer.apiController
{
    public class LogDownloadApiController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage FetchParent(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
            {
                var ex = new Exception("Directory does not exist.");
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, ex);
            }

            var parent = Directory.GetParent(dir);
            if (parent == null || !parent.Exists)
            {
                var ex = new Exception("Directory does not exist.");
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, ex);
            }

            return Fetch(parent.FullName);
        }

        [HttpGet]
        public HttpResponseMessage Fetch(string dir = "")
        {
            if (string.IsNullOrWhiteSpace(dir))
                dir = ColloSysParam.WebParams.LogPath;

            if (!Directory.Exists(dir))
            {
                var ex = new Exception("Directory does not exist.");
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, ex);
            }

            var viewModel = new LogFileViewModel
                {
                    Path = dir,
                    Drive = Directory.GetDirectoryRoot(dir),
                    LogFiles = new List<LogFileModel>(),
                    Directories = new List<LogFileModel>()
                };

            GetFiles(viewModel);
            GetDirectory(viewModel);

            return Request.CreateResponse(HttpStatusCode.Created, viewModel);
        }

        [HttpGet]
        public HttpResponseMessage GetDrives()
        {
            var drivers = DriveInfo.GetDrives();
            var drivesarray = drivers.Select(x => x.Name).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, drivesarray);
        }

        private static void GetDirectory(LogFileViewModel viewModel)
        {
            var dirList = Directory.GetDirectories(viewModel.Path)
                                   .Select(x => new DirectoryInfo(x))
                                   .OrderBy(x => x.Name)
                                   .ToList();

            viewModel.Directories = dirList
                .Select(file => file != null
                                    ? new LogFileModel
                                        {
                                            Path = string.Empty,
                                            FullName = file.FullName,
                                            Name = file.Name
                                        }
                                    : null).ToList();
        }

        private static void GetFiles(LogFileViewModel viewModel)
        {
            var logFileList = Directory.GetFiles(viewModel.Path)
                                       .Select(logfile => new FileInfo(logfile))
                                       .OrderByDescending(x => x.CreationTime)
                                       .ToList();

            viewModel.LogFiles = logFileList
                .Select(file => file.Directory != null
                                    ? new LogFileModel
                                        {
                                            Path = file.Directory.FullName,
                                            FullName = file.FullName,
                                            Name = file.Name
                                        }
                                    : null).ToList();
        }
    }
}
