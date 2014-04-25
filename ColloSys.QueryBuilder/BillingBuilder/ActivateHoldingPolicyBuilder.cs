using System.Collections.Generic;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Mapping;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;

namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class ActivateHoldingPolicyBuilder : Repository<ActivateHoldingPolicy>
    {
        [Transaction]
        public IEnumerable<ActivateHoldingPolicy> GetAllActivateHoldingPolicies()
        {
            return SessionManager.GetCurrentSession().
                QueryOver<ActivateHoldingPolicy>()
                .Fetch(x => x.Stakeholder).Eager
                .Fetch(x => x.HoldingPolicy).Eager.List();
        }
    }
}