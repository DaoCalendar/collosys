using System.Collections.Generic;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;

namespace AngularUI.Stakeholder.view
{
    public class ViewStakeApiController : BaseApiController<Stakeholders>
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();

        [HttpGet]
        public IEnumerable<Stakeholders> GetAllStakeHolders()
        {
            return StakeQuery.GetAllStakeholders();
        }
    }
}