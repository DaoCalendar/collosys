#region references

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.webapis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.UserInterface.Areas.ErrorCorrection.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using NLog;
using UserInterfaceAngular.NgGrid;

#endregion


namespace AngularUI.FileUpload.errorapproval
{
    public class ApproveEditedErrorDataApiController : ApiController
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region Get
        [HttpGet]
        [HttpTransaction2]
        public IEnumerable<FileScheduler> GetFileSchedulers()
        {
            var session = SessionManager.GetCurrentSession();
            var getFileScheduler = session.QueryOver<FileScheduler>()
                                          .Fetch(x => x.FileDetail).Eager
                                          .And(x => x.UploadStatus == ColloSysEnums.UploadStatus.DoneWithError)
                                          .List();
            return getFileScheduler;
        }

        [HttpGet]
        [HttpTransaction2]
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
            catch (HibernateException e)
            {
                _log.Error("ApiController: Error in get: " + e);
                throw new Exception(e.Message, e);
            }
            catch (Exception exception)
            {
                _log.ErrorException("GetFileDetails : ", exception);
                throw new Exception(exception.Message, exception);
            }
        }

        [HttpGet]
        [HttpTransaction2]
        public NgGridOptions GetNgGridOptions(Guid fileDetailId)
        {
            try
            {
                return ErrorViewModel.GetApproverErrorNgGrid(fileDetailId);
            }

            catch (NullReferenceException e)
            {
                _log.Error("GetNgGridOptions: Wrong id: " + e);
                throw new Exception(e.Message, e);
            }
            catch (HibernateException e)
            {
                _log.Error("GetNgGridOptions: Error in Get: " + e);
                throw new Exception(e.Message, e);
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
        [HttpTransaction2(Persist = true)]
        public HttpResponseMessage PostRow(JObject value)
        {
            try
            {
                var dictionary = value.ToObject<Dictionary<string, object>>();
                var approved = Convert.ToBoolean(dictionary["approved"]);
                var fileAliasName = dictionary["fileAliasName"].ToString();
                var tableName = dictionary["tableName"].ToString();
                var errorData = JObject.FromObject(dictionary["data"])
                                       .ToObject<Dictionary<string, object>>();

                if (approved)
                {
                    ErrorViewModel.ApproveErrorData(fileAliasName, tableName, errorData);
                }
                else
                {
                    ErrorViewModel.RejectErrorData(tableName, errorData);
                }

                _log.Info("Row Approved");
                return Request.CreateResponse(HttpStatusCode.Created, "Data Saved");
            }
            catch (NullReferenceException e)
            {
                _log.Error("ApiController: Error in Post: " + e);
                return Request.CreateResponse(HttpStatusCode.BadRequest, e);
            }
            catch (InvalidDataException e)
            {
                _log.Error("ApiController: Error in Post: " + e);
                return Request.CreateResponse(HttpStatusCode.BadRequest, e);
            }
            catch (HibernateException e)
            {
                _log.Error("ApiController: Error in Post: " + e);
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            }
            catch (Exception ex)
            {
                _log.ErrorException("PostRow : ", ex);
                return Request.
                    CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [HttpTransaction2(Persist = true)]
        public HttpResponseMessage PostRows(JObject value)
        {
            try
            {
                var dictionary = value.ToObject<Dictionary<string, object>>();
                var approved = Convert.ToBoolean(dictionary["approved"]);
                var fileAliasName = dictionary["fileAliasName"].ToString();
                var tableName = dictionary["tableName"].ToString();

                var errorDataRows = JsonConvert.DeserializeObject<IEnumerable<object>>
                    (dictionary["data"].ToString())
                    .ToArray();

                foreach (Dictionary<string, object> errorDataRow in errorDataRows)
                {
                    if (approved)
                    {
                        ErrorViewModel.ApproveErrorData(fileAliasName, tableName, errorDataRow);
                    }
                    else
                    {
                        ErrorViewModel.RejectErrorData(tableName, errorDataRow);
                    }
                }

                _log.Info(string.Format("Selected {0} Rows Approved", errorDataRows.Count()));
                return Request.CreateResponse(HttpStatusCode.Created, "Data Saved");
            }

            catch (NullReferenceException e)
            {
                _log.Error("ApiController: Error in Post: " + e);
                return Request.CreateResponse(HttpStatusCode.BadRequest, e);
            }
            catch (InvalidDataException e)
            {
                _log.Error("ApiController: Error in Post: " + e);
                return Request.CreateResponse(HttpStatusCode.BadRequest, e);
            }
            catch (HibernateException e)
            {
                _log.Error("ApiController: Error in Post: " + e);
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, e);
            }
            catch (Exception ex)
            {
                _log.ErrorException("PostRows : ", ex);
                return Request.
                    CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        #endregion
    }
}