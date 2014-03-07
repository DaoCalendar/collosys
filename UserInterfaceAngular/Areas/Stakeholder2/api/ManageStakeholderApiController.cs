using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Linq;

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
            var query = StakeQuery.DefaultQuery().Where(x => x.Name.StartsWith(name));
            
            var data = StakeQuery.ExecuteQuery(query).Take(10).ToList(); 
            return data;
        }
    }
}
