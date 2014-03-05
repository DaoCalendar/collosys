﻿#region references

using System.Globalization;
using System.Net;
using System.Net.Http;
using ColloSys.DataLayer.Domain;
using System;
using System.Collections.Generic;
using System.Web.Http;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.Shared.ExcelWriter;
using ColloSys.UserInterface.Areas.FileUploader.Models;
using ColloSys.UserInterface.Shared.Attributes;

#endregion

namespace UserInterfaceAngular.app
{
    public class FileStatusApiController : ApiController
    {
        [HttpGet]
        [HttpTransaction]
        public IEnumerable<FileScheduler> Get()
        {
            return FileStatusHelper.GetOneWeekScheduledList();
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<FileScheduler> GetStatusByDate(string fromDate,string toDate)
        {
            var fromdate = Convert.ToDateTime(fromDate);

            var todate = Convert.ToDateTime(toDate);

            return FileStatusHelper.GetScheduledList(fromdate, todate);
        }

        [HttpDelete]
        [HttpTransaction(Persist = true)]
        public void Delete(Guid id)
        {
            var session = SessionManager.GetCurrentSession();
            var entity = session.Load<FileScheduler>(id);
            SessionManager.GetCurrentSession().Delete(entity);
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
    }
}