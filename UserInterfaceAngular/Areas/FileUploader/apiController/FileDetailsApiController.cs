#region references

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.FileUploader;
using ColloSys.QueryBuilder.FileUploadBuilder;
using ColloSys.UserInterface.Areas.FileUploader.Models;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate;
using NLog;

#endregion


namespace UserInterfaceAngular.app
{
    public class FileDetailsApiController : BaseApiController<FileDetail>
    {
        readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [HttpTransaction]
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
        [HttpTransaction]
        public IEnumerable<FileDetail> GetFileDetailList()
        {
            return new FileDetailBuilder().GetAll();
        }
    }
}