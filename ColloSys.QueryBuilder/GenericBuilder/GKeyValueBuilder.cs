#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.GenericBuilder
{
    public class GKeyValueBuilder : QueryBuilder<GKeyValue>
    {
        public override QueryOver<GKeyValue, GKeyValue> DefaultQuery()
        {
            return QueryOver.Of<GKeyValue>();
        }

        [Transaction]
        public IEnumerable<GKeyValue> ForStakeholders()
        {
            return SessionManager.GetCurrentSession().QueryOver<GKeyValue>()
                                 .Where(x => x.Area == ColloSysEnums.Activities.Stakeholder)
                                 .List<GKeyValue>();
        }
    }
}