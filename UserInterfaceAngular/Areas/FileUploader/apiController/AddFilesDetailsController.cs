﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Areas.FileUploader.Models;
using ColloSys.UserInterface.Shared.Attributes;
using Glimpse.AspNet.Tab;
using NHibernate;
using NHibernate.Transform;
using NLog;




namespace ColloSys.UserInterface.Areas.FileUploader.apiController
{
    public class AddFilesDetailsController : ApiController //: BaseApiController<FileMapping>
    {

        readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected ISession Session
        {
            get { return SessionManager.GetCurrentSession(); }
        }

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
        public IEnumerable<string> GetValueTypes()
        {
            return Enum.GetNames(typeof(ColloSysEnums.FileMappingValueType));
        }

        [HttpGet]
        public IEnumerable<string> GetFileTypes()
        {
            return from t in typeof(CLiner).Assembly.GetTypes()
                   where typeof(UploadableEntity).IsAssignableFrom(t) && !t.IsAbstract
                   select t.Name;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<FileDetail> GetFileDetails()
        {
            try
            {
                return (Session.QueryOver<FileDetail>()
                    .Where(c => c.ScbSystems == ScbEnums.ScbSystems.EBBS || c.ScbSystems == ScbEnums.ScbSystems.RLS).List());
            }
            catch (Exception exception)
            {
                _log.ErrorException("GetFileDetails : ", exception);
                throw new Exception(exception.Message, exception);
            }
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<FileMapping> GetFileMappings(string actualTableName, string tempTableName)
        {
            try
            {
                return FileMappingViewModel.GetFileMappings(actualTableName, tempTableName);
            }
            catch (Exception exception)
            {
                _log.ErrorException("GetFileMappings : ", exception);
                throw new Exception(exception.Message, exception);
            }
        }

        // GET api/<controller>
        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetFileColumns(string aliasName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(aliasName))
                    throw new NullReferenceException("File Name Can not be Null or Empty");

                var alias = (ColloSysEnums.FileAliasName)Enum.Parse(typeof(ColloSysEnums.FileAliasName), aliasName);

                var session = SessionManager.GetCurrentSession();
                var getFileDetail = session.QueryOver<FileDetail>()
                                           .Where(x => x.AliasName == alias)
                                           .Fetch(x => x.FileColumns).Eager
                                           .TransformUsing(Transformers.DistinctRootEntity)
                                           .SingleOrDefault();

                _logger.Info("View File Details");
                return Request.CreateResponse(HttpStatusCode.Created, getFileDetail);
            }
            catch (NullReferenceException e)
            {
                _logger.Error("Get Method All File Columns of a selected File:" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
            catch (ArgumentNullException e)
            {
                _logger.Error("Get Method All File Columns of a selected File:" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
            catch (HibernateException e)
            {
                _logger.Error("Get Method All File Columns of a selected File:" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }


        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage Fetchit()
        {
            try
            {
                var data = new FileColumnViewModel();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (HibernateException e)
            {
                _logger.Error("Get Method All Files:" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
            {
                _logger.Error("Get Method All Files:" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage SaveAllColumns(FileDetail fileDetail)
        {
            try
            {
                if (fileDetail == null || fileDetail.FileColumns == null || fileDetail.FileColumns.Count == 0)
                    throw new NullReferenceException("File Detail or File columns can not be null");

                foreach (var column in fileDetail.FileColumns)
                {
                    column.FileDetail = fileDetail;
                }
                Session.SaveOrUpdate(fileDetail);

                return Request.CreateResponse(HttpStatusCode.Created, fileDetail);
            }
            catch (NullReferenceException e)
            {
                _logger.Error("Save All File Columns:" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
            catch (HibernateException e)
            {
                _logger.Error("Save All File Columns:" + e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpPut]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage PutFileColumn(IEnumerable<FileColumn> fileColumns)
        {
            var enumerable = fileColumns as FileColumn[] ?? fileColumns.ToArray();

            var id = enumerable[0].FileDetail.Id;
            var temp = SessionManager.GetCurrentSession()
                                     .QueryOver<FileDetail>().Where(c => c.Id == id)
                                     .SingleOrDefault();
            foreach (var fileColumn in enumerable)
            {
                fileColumn.FileDetail = temp;
                Session.Merge(fileColumn);
            }

            return Request.CreateResponse(HttpStatusCode.Created, enumerable);
        }
    }
}
