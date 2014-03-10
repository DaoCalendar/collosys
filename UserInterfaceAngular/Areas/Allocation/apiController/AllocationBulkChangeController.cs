#region references

using System.Collections.Generic;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Areas.Allocation.ViewModels;
using ColloSys.UserInterface.Shared.Attributes;
using System.Linq;

#endregion

//stakeholders calls changed
namespace ColloSys.UserInterface.Areas.Allocation.apiController
{
    public class AllocationBulkChangeController : ApiController
    {
        private static readonly StakeQueryBuilder StakeQuery =new StakeQueryBuilder();
        private static readonly ProductConfigBuilder ProductConfigBuilder=new ProductConfigBuilder(); 

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<ScbEnums.Products> GetProducts()
        {
            return ProductConfigBuilder.GetProducts();
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
