using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Areas.Allocation.ViewModels;
using ColloSys.UserInterface.Shared.Attributes;

namespace ColloSys.UserInterface.Areas.Allocation.apiController
{
    public class AllocationApproveApiController : ApiController
    {
        [HttpGet]
        [HttpTransaction]
        public IEnumerable<ScbEnums.Products> GetProducts()
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var data = session.QueryOver<ProductConfig>()
                                      .Select(x => x.Product)
                                      .List<ScbEnums.Products>();
                    trans.Rollback();
                    return data;
                }
            }

        }

        [HttpPost]
        [HttpTransaction]
        public List<AllocationApprove> GetAllocations(DataModel2 model)
        {
            if (model.AllocDate == DateTime.MinValue)
            {
                model.AllocDate = DateTime.Now;
            }

            var allocListModel = new AllocationApprove();
            var list = allocListModel.GetAllocations(model);
            return list;
        }

        [HttpPost]
        [HttpTransaction]
        public bool ApproveAllocations(List<AllocationApprove> list)
        {
            return true;
        }
    }
}
