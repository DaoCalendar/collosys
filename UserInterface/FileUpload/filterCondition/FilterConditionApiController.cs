using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.FileUploader;
using ColloSys.QueryBuilder.ClientDataBuilder;
using ColloSys.QueryBuilder.FileUploadBuilder;

namespace AngularUI.FileUpload.clientData
{
    public class FilterConditionApiController : BaseApiController<FilterCondition>
    {
        private static readonly FileDetailBuilder FileDetailBuilder = new FileDetailBuilder();
        private static readonly FileColumnBuilder FileColumnBuilder = new FileColumnBuilder();
        private static readonly FilterConditionBuilder FilterConditionBuilder=new FilterConditionBuilder();

        [HttpGet]
        public IEnumerable<FileDetail> GetFiledetails()
        {
            var data = FileDetailBuilder.GetAll();
            return data;
        }

        [HttpGet]
        public HttpResponseMessage GetFileColumnData(Guid fileDetailId)
        {
            var data = FileColumnBuilder.OnFileDetailId(fileDetailId);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetFilterConditionDeatils(Guid fileGuid)
        {
            var data = FilterConditionBuilder.OnAliasNameChange(fileGuid);
            return Request.CreateResponse(HttpStatusCode.OK,data);
        }


        //[HttpPost]
        //protected override FilterCondition BasePost(FilterCondition obj)
        //{
        //    foreach (var fcondition in obj.Fconditions)
        //    {
        //        fcondition.FilterCondition = obj;
        //    }
        //    FilterConditionBuilder.Save(obj);
        //    return obj;
        //}

    }
}
