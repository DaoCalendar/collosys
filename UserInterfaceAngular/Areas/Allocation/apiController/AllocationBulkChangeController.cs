#region references

using System.Collections.Generic;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Areas.Allocation.ViewModels;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.SqlCommand;

#endregion


namespace ColloSys.UserInterface.Areas.Allocation.apiController
{
    public class AllocationBulkChangeController : ApiController
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

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetStakeholders()
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    Stakeholders stake = null;
                    StkhWorking work = null;
                    StkhHierarchy hierarchy = null;
                    var data = session.QueryOver<Stakeholders>(() => stake)
                                      .Fetch(x => x.Hierarchy).Eager
                                      .JoinAlias(() => stake.Hierarchy, () => hierarchy, JoinType.InnerJoin)
                                      .Where(() => hierarchy.IsInAllocation)
                                      .List();
                    trans.Rollback();
                    return data;
                }
            }
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
