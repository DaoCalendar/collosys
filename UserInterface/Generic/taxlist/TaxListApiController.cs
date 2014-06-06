using AngularUI.Shared.apis;
using ColloSys.DataLayer.Generic;
using ColloSys.QueryBuilder.GenericBuilder;

namespace AngularUI.Generic.taxlist
{
    public class TaxListApiController : BaseApiController<GTaxesList>
    {
        private static readonly GTaxesListBuilder GTaxesListBuilder=new GTaxesListBuilder();

        //[HttpPost]
        //public HttpResponseMessage Save(GTaxesList gTaxesList)
        //{
        //    GTaxesListBuilder.Save(gTaxesList);
        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        //[HttpGet]
        //public HttpResponseMessage GetAll()
        //{
        //    var data = GTaxesListBuilder.GetAll();
        //    return Request.CreateResponse(HttpStatusCode.OK, data);
        //}

    }
}
