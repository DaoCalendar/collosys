#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.AllocationBuilder
{
    public class AllocConditionBuilder : Repository<AllocCondition>
    {
        public override QueryOver<AllocCondition, AllocCondition> ApplyRelations()
        {
            return QueryOver.Of<AllocCondition>();
        }

        [Transaction]
        public IEnumerable<AllocCondition> OnSubpolicyId(Guid subpolicyId)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<AllocCondition>()
                                 .Where(x => x.AllocSubpolicy.Id == subpolicyId)
                                 .List();
        }
    }
}