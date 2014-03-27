#region references

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.FileUploader;
using ColloSys.UserInterface.Areas.FileUploader.Models;
using NHibernate;
using NLog;

#endregion


namespace AngularUI.FileUpload.filedetail
{
    public class FileDetailsApiController : BaseApiController<FileDetail>
    {
        readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        
        public HttpResponseMessage Fetch()
        {
            try
            {
                var data = new FileDetailViewModel();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (HibernateException e)
            {
                _logger.Error("Get All File Details:" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
            {
                _logger.Error("Get All File Details:" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        
        public IEnumerable<FileDetail> GetFileDetailList()
        {
            return Session.QueryOver<FileDetail>().Select(x=>x).List();
        }
    }
}