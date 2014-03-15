#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.FileUploadBuilder;
using ColloSys.UserInterface.Areas.FileUploader.Models;
using ColloSys.UserInterface.Shared.Attributes;

#endregion

namespace AngularUI.FileUpload.filestatus
{
    public class FileStatusApiController : ApiController
    {
        private static readonly FileSchedulerBuilder FileSchedulerBuilder = new FileSchedulerBuilder();
        [HttpGet]
        [HttpTransaction]
        public IEnumerable<FileScheduler> Get()
        {
            return FileStatusHelper.GetOneWeekScheduledList();
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<FileScheduler> GetStatusByDate(string fromDate, string toDate)
        {
            var fromdate = Convert.ToDateTime(fromDate);

            var todate = Convert.ToDateTime(toDate);

            return FileStatusHelper.GetScheduledList(fromdate, todate);
        }

        [HttpDelete]
        [HttpTransaction(Persist = true)]
        public void Delete(Guid id)
        {
            var entity = FileSchedulerBuilder.Load(id);
            FileSchedulerBuilder.Delete(entity);
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public FileScheduler ImmidiateFileSchedule(FileScheduler fileScheduler)
        {
            return FileStatusHelper.ReScheduleFile(fileScheduler);
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public FileScheduler RetryUpload(FileScheduler file)
        {
            return FileStatusHelper.RetryUpload(file.Id);
        }

        [HttpPost]
        [HttpTransaction]
        public HttpResponseMessage DownloadFile(IEnumerable<FileScheduler> scheduler)
        {
            try
            {
                var fileinfo = FileStatusHelper.DownloadFile(scheduler);
                return Request.CreateResponse(HttpStatusCode.OK, fileinfo.FullName);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, exception.Message);
            }
        }

        public class Helper
        {
            public string Filename;
        }
    }
}