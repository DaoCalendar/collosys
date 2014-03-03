using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Transform;

namespace ColloSys.UserInterface.Areas.Billing.apiController
{
    public class AddhocPayoutsApiController : BaseApiController<BillAdhoc>
    {
        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetProducts()
        {
            var data = Session.QueryOver<ProductConfig>().Select(x => x.Product).List<ScbEnums.Products>();
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetAdhocdata(ScbEnums.Products products)
        {
            var data = Session.QueryOver<BillAdhoc>()
                .Fetch(x=>x.Stakeholder).Eager
                .Where(x => x.Products == products)
                .List<BillAdhoc>();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetStakeHolders(ScbEnums.Products products)
        {
            Stakeholders stakeholders = null;
            StkhWorking working = null;
            StkhHierarchy hierarchy = null;
            var session = SessionManager.GetCurrentSession();
            var data = session.QueryOver<Stakeholders>(() => stakeholders)
                              .Fetch(x => x.StkhWorkings).Eager
                              .JoinQueryOver(() => stakeholders.StkhWorkings, () => working)
                              .JoinQueryOver(() => stakeholders.Hierarchy, () => hierarchy)
                              .Where(() => working.Products == products)
                              .And(() => hierarchy.IsInAllocation)
                              .And(() => hierarchy.IsInField)
                              .TransformUsing(Transformers.DistinctRootEntity)
                              .List<Stakeholders>();
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

       
    }
}
