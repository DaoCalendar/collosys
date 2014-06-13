using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class HierarchyQueryBuilder : Repository<StkhHierarchy>
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
        public IEnumerable<StkhHierarchy> GetAllHierarchies()
        {
            var Session = SessionManager.GetCurrentSession();
            return Session.QueryOver<StkhHierarchy>()
                          .Where(x => x.Hierarchy != "Developer")
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

        public StkhHierarchy OnHierarchyId(Guid id)
        {
            var session = SessionManager.GetCurrentSession();

            var data = session.Query<StkhHierarchy>().Single(x => x.Id == id);
            return data;
        }

        public override QueryOver<StkhHierarchy, StkhHierarchy> ApplyRelations()
        {
            return QueryOver.Of<StkhHierarchy>()
                            .Fetch(x => x.GPermissions).Eager
                            .TransformUsing(Transformers.DistinctRootEntity);
        }
    }
}