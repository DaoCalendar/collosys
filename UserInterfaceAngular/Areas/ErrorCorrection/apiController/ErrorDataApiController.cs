#region references

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Areas.ErrorCorrection.ViewModels;
using ColloSys.UserInterface.Shared.Attributes;
using NLog;
using Newtonsoft.Json.Linq;
using UserInterfaceAngular.NgGrid;

#endregion

namespace UserInterfaceAngular.app
{
    public class ErrorDataApiController : ApiController
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region Get

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<FileDetail> GetFileDetails()
        {
            try
            {
                var session = SessionManager.GetCurrentSession();
                var getEbbsRlsFileDetails = (session.QueryOver<FileDetail>()
                    .Where(c => c.ScbSystems == ScbEnums.ScbSystems.EBBS
                        || c.ScbSystems == ScbEnums.ScbSystems.RLS).List());

                return getEbbsRlsFileDetails;
            }
            catch (Exception exception)
            {
                _log.ErrorException("GetFileDetails : ", exception);
                throw new Exception(exception.Message, exception);
            }
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<FileScheduler> GetFileSchedulers(Guid fileDetailId)
        {
            try
            {
                var session = SessionManager.GetCurrentSession();
                FileScheduler scheduler = null;
                FileDetail detail = null;
                // ReSharper disable ImplicitlyCapturedClosure
                var getFileScheduler = session.QueryOver(() => scheduler)
                                              .JoinAlias(() => scheduler.FileDetail, () => detail)
                                              .Where(() => scheduler.FileDetail.Id == fileDetailId)
                                              .And(() =>
                                                  scheduler.UploadStatus == ColloSysEnums.UploadStatus.DoneWithError)
                                              .And(() =>
                                                   (detail.Frequency != ColloSysEnums.FileFrequency.Daily &&
                                                    scheduler.FileDate > DateTime.Today.AddDays(-100))
                                                   ||
                                                   (detail.Frequency == ColloSysEnums.FileFrequency.Daily &&
                                                    scheduler.FileDate > DateTime.Today.AddDays(-31)))
                                              .List();
                // ReSharper restore ImplicitlyCapturedClosure

                return getFileScheduler;
            }
            catch (Exception exception)
            {
                _log.ErrorException("GetFileDetails : ", exception);
                throw new Exception(exception.Message, exception);
            }
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<FileScheduler> GetFileSchedulers()
        {
            try
            {
                var session = SessionManager.GetCurrentSession();
                FileScheduler scheduler = null;
                FileDetail detail = null;
                // ReSharper disable ImplicitlyCapturedClosure
                var getFileScheduler = session.QueryOver(() => scheduler)
                                              .JoinAlias(() => scheduler.FileDetail, () => detail)
                                              .Fetch(x=>x.FileDetail).Eager
                                              .And(() =>
                                                  scheduler.UploadStatus == ColloSysEnums.UploadStatus.DoneWithError)
                                              .And(() =>
                                                   (detail.Frequency != ColloSysEnums.FileFrequency.Daily &&
                                                    scheduler.FileDate > DateTime.Today.AddDays(-100))
                                                   ||
                                                   (detail.Frequency == ColloSysEnums.FileFrequency.Daily &&
                                                    scheduler.FileDate > DateTime.Today.AddDays(-31)))
                                              .List();
                // ReSharper restore ImplicitlyCapturedClosure

                return getFileScheduler;
            }
            catch (Exception exception)
            {
                _log.ErrorException("GetFileDetails : ", exception);
                throw new Exception(exception.Message, exception);
            }
        }

        [HttpGet]
        [HttpTransaction]
        public NgGridOptions GetNgGridOptions(Guid fileSchedulerId)
        {
            try
            {
                return ErrorViewModel.GetErrorNgGrid(fileSchedulerId);
            }
            catch (Exception exception)
            {
                _log.ErrorException("GetNgGridOptions : ", exception);
                throw new Exception(exception.Message, exception);
            }
        }

        #endregion

        #region Post

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage Post(JObject value)
        {
            try
            {
                var dictionary = value.ToObject<Dictionary<string, object>>();
                var validate = Convert.ToBoolean(dictionary["validate"]);
                var fileAliasName = dictionary["fileAliasName"].ToString();
                var tableName = dictionary["tableName"].ToString();
                var errorData = JObject.FromObject(dictionary["data"])
                                       .ToObject<Dictionary<string, object>>();

                ErrorViewModel.EditErrorData(validate, fileAliasName, tableName, errorData);
                _log.Info("Error Data Edited and Saved");
                return Request.CreateResponse(HttpStatusCode.Created, "Data Saved");
            }
            catch (Exception ex)
            {
                _log.ErrorException("Post : ", ex);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage RetryErrorRows(FileScheduler fileScheduler)
        {
            try
            {
                ErrorViewModel.RetryErrorRows(fileScheduler);
                return Request.CreateResponse(HttpStatusCode.Created, "Retry Done");
            }
            catch (Exception ex)
            {
                _log.ErrorException("RetryErrorRows : ", ex);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpDelete]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage Delete(string tableName, Guid id)
        {
            try
            {
                ErrorViewModel.DeleteErrorData(tableName, id);
                _log.Info("Error Data Deleted");
                return Request.CreateResponse(HttpStatusCode.Created, "Data Deleted");
            }
            catch (Exception ex)
            {
                _log.ErrorException("Delete : ", ex);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        #endregion
    }
}