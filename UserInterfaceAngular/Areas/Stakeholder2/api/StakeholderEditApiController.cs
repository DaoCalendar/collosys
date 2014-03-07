#region references

using System.Linq;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Shared.Attributes;

#endregion


//stakeholders calls changed
namespace ColloSys.UserInterface.Areas.Stakeholder2.api
{
    public class StakeholderEditApiController : ApiController
    {
        private readonly StakeQueryBuilder _stakeQuery = new StakeQueryBuilder();
        [HttpGet]
        [HttpTransaction]
        public Stakeholders GetStakeholderEditMode()
        {
            var query = _stakeQuery.DefaultQuery();
            var stake = _stakeQuery.ExecuteQuery(query).FirstOrDefault();
            return stake;
        }
    }
}
