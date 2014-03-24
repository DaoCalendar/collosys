#region references

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Shared.Attributes;

#endregion

//stakeholders calls changed
namespace ColloSys.UserInterface.Areas.Stakeholder2.api
{
    public class ManageStakeholderApiController : ApiController
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();
        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetStakeholders(string name)
        {
            var query = StakeQuery.ApplyRelations().Where(x => x.Name.StartsWith(name));
            
            var data = StakeQuery.Execute(query).Take(10).ToList(); 
            return data;
        }
    }
}
