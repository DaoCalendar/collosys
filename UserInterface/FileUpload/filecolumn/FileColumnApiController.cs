#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.UserInterface.Areas.FileUploader.Models;
using NHibernate;
using NHibernate.Transform;
using NLog;

#endregion

namespace UserInterfaceAngular.app
{
    public class FileColumnApiController : BaseApiController<FileColumn>
    {
        readonly Logger _logger = LogManager.GetCurrentClassLogger();

        // GET api/<controller>
        [HttpGet]
        
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
        
        public HttpResponseMessage Fetch()
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

        protected override FileColumn BasePost(FileColumn fileColumn)
        {
            if (fileColumn.StartDate >= fileColumn.EndDate)
                throw new InvalidDataException("Start Date can not be Greater than or Equal to End Date");

            var fileDetailId = fileColumn.FileDetail.Id;
            fileColumn.FileDetail = null;
            var session = SessionManager.GetCurrentSession();
            fileColumn.FileDetail = session.QueryOver<FileDetail>().Where(c => c.Id == fileDetailId).SingleOrDefault();
            SessionManager.GetCurrentSession().SaveOrUpdate(fileColumn);
            return fileColumn;
        }

        [HttpPost]
        
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