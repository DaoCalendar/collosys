using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Generic;
using ColloSys.QueryBuilder.GenericBuilder;

namespace AngularUI.Generic.taxmaster
{
    public class TaxMasterApiController : BaseApiController<GTaxDetail>
    {
        private static readonly GPincodeBuilder GPincodeBuilder = new GPincodeBuilder();
        private static readonly GTaxesListBuilder GTaxesListBuilder=new GTaxesListBuilder();
        private static readonly GTaxDetailsBuilder GTaxDetailsBuilder=new GTaxDetailsBuilder();

        [HttpGet]
        public HttpResponseMessage States()
        {
            var data = GPincodeBuilder.StateList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GTaxList()
        {
            var data = GTaxesListBuilder.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage TaxMasterList()
        {
            var data = GTaxDetailsBuilder.GetAllWithRef();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
        
        [HttpGet]
        public HttpResponseMessage GetData(Guid id)
        {
            var data = Session.QueryOver<GTaxDetail>()
                              .Where(x => x.Id == id)
                              .Fetch(x => x.GTaxesList).Eager
                              .SingleOrDefault();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}

