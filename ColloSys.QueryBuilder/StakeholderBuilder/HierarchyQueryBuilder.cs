using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.Linq;
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

        [Transaction]
        public IEnumerable<StkhHierarchy> OnDesignationHierarchy(string designation, string hierarchy)
        {
            var session = SessionManager.GetCurrentSession();

            var data = session.Query<StkhHierarchy>()
                              .Where(x => x.Designation == designation && x.Hierarchy == hierarchy)
                              .Select(x => x).ToList();
            return data;
        }

        public override QueryOver<StkhHierarchy,StkhHierarchy> DefaultQuery()
        {
            return QueryOver.Of<StkhHierarchy>()
                            .Fetch(x => x.GPermissions).Eager
                            .TransformUsing(Transformers.DistinctRootEntity);
        }
    }
}