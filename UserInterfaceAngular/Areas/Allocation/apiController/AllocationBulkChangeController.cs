#region references

using System.Collections.Generic;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Areas.Allocation.ViewModels;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.SqlCommand;

#endregion

//stakeholders calls changed
namespace ColloSys.UserInterface.Areas.Allocation.apiController
{
    public class AllocationBulkChangeController : ApiController
    {
        private static readonly StakeQueryBuilder StakeQuery =new StakeQueryBuilder();
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

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetStakeholders()
        {
            return StakeQuery.AllocationBulkChange();
        }

        [HttpPost]
        [HttpTransaction]
        public BulkAllocationModel GetAllocations(BulkAllocationModel model)
        {
            return model.GetAllocations(model);
        }

        [HttpPost]
        [HttpTransaction]
        public BulkAllocationModel ChangeAllocations(BulkAllocationModel model)
        {
            model.SaveAllocationsWithoutChurn(model);
            return model;
        }
    }
}
