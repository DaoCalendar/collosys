using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.webapis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Areas.ErrorCorrection.ViewModels;
using Newtonsoft.Json.Linq;
using UserInterfaceAngular.NgGrid;

namespace AngularUI.FileUpload.errorcorrection
{
    public class ErrorDataApiController : ApiController
    {
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
        public NgGridOptions GetNgGridOptions(Guid fileSchedulerId)
        {
            try
            {
                return ErrorViewModel.GetErrorNgGrid(fileSchedulerId);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message, exception);
            }
        }

        [HttpPost]
        [HttpTransaction2(Persist = true)]
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
                return Request.CreateResponse(HttpStatusCode.Created, "Data Saved");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [HttpTransaction2(Persist = true)]
        public HttpResponseMessage RetryErrorRows(FileScheduler fileScheduler)
        {
            try
            {
                ErrorViewModel.RetryErrorRows(fileScheduler);
                return Request.CreateResponse(HttpStatusCode.Created, "Retry Done");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpDelete]
        [HttpTransaction2(Persist = true)]
        public HttpResponseMessage Delete(string tableName, Guid id)
        {
            try
            {
                ErrorViewModel.DeleteErrorData(tableName, id);
                return Request.CreateResponse(HttpStatusCode.Created, "Data Deleted");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}


//.And(x =>
//     (x.FileDetail.Frequency != ColloSysEnums.FileFrequency.Daily &&
//      x.FileDate > DateTime.Today.AddDays(-100))
//     ||
//     (x.FileDetail.Frequency == ColloSysEnums.FileFrequency.Daily &&
//      x.FileDate > DateTime.Today.AddDays(-31)))

//[HttpGet]
//public IEnumerable<FileScheduler> GetFileSchedulers(Guid fileDetailId)
//{
//    try
//    {
//        var session = SessionManager.GetCurrentSession();
//        FileScheduler scheduler = null;
//        FileDetail detail = null;
//        // ReSharper disable ImplicitlyCapturedClosure
//        var getFileScheduler = session.QueryOver(() => scheduler)
//                                      .JoinAlias(() => scheduler.FileDetail, () => detail)
//                                      .Where(() => scheduler.FileDetail.Id == fileDetailId)
//                                      .And(() =>
//                                          scheduler.UploadStatus == ColloSysEnums.UploadStatus.DoneWithError)
//                                      .And(() =>
//                                           (detail.Frequency != ColloSysEnums.FileFrequency.Daily &&
//                                            scheduler.FileDate > DateTime.Today.AddDays(-100))
//                                           ||
//                                           (detail.Frequency == ColloSysEnums.FileFrequency.Daily &&
//                                            scheduler.FileDate > DateTime.Today.AddDays(-31)))
//                                      .List();
//        // ReSharper restore ImplicitlyCapturedClosure

//        return getFileScheduler;
//    }
//    catch (Exception exception)
//    {
//        throw new Exception(exception.Message, exception);
//    }
//}


//[HttpGet]
//        public IEnumerable<FileDetail> GetFileDetails()
//        {
//            var session = SessionManager.GetCurrentSession();
//            var getEbbsRlsFileDetails = (session.QueryOver<FileDetail>()
//                .Where(c => c.ScbSystems == ScbEnums.ScbSystems.EBBS
//                    || c.ScbSystems == ScbEnums.ScbSystems.RLS).List());

//            return getEbbsRlsFileDetails;
//        }
