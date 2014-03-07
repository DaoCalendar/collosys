using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

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

        public override QueryOver<StkhHierarchy,StkhHierarchy> DefaultQuery()
        {
            return QueryOver.Of<StkhHierarchy>()
                            .Fetch(x => x.GPermissions).Eager
                            .TransformUsing(Transformers.DistinctRootEntity);
        }
    }
}