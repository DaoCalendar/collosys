using System.Collections.Generic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Mapping;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;

namespace ColloSys.QueryBuilder.BillingBuilder
{
    public class ActivateHoldingPolicyBuilder : Repository<StkhHoldingPolicy>
    {
        [Transaction]
        public IEnumerable<StkhHoldingPolicy> GetAllActivateHoldingPolicies()
        {
            return SessionManager.GetCurrentSession().
                QueryOver<StkhHoldingPolicy>()
                .Fetch(x => x.Stakeholder).Eager
                .Fetch(x => x.HoldingPolicy).Eager.List();
        }
    }
}