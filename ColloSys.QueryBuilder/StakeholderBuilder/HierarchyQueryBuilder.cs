using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate;

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class HierarchyQueryBuilder:QueryBuilder<StkhHierarchy>
    {
        [Transaction]
        public IEnumerable<StkhHierarchy> ExceptDeveloperExternal()
        {
            var Session = SessionManager.GetCurrentSession();
            return Session.QueryOver<StkhHierarchy>()
                          .Where(x => x.Hierarchy != "Developer" && x.Hierarchy != "External")
                          .List();
        }
        public override IQueryOver<StkhHierarchy> DefaultQuery(ISession session)
        {
            return null;
        }
    }
}