using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Performance;
using ColloSys.DataLayer.Performance.PerformanceParameter;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Linq;

namespace ColloSys.QueryBuilder.PerformanceBuilder
{
    public class model
    {
        public virtual Guid Id { get; set; }
        public virtual string Designation { get; set; }
        public virtual StkhHierarchy Hierarchy { get; set; }
    }

    public class PerformanceParamBuilder:Repository<PerformanceParams>
    {
        [Transaction]
       public IList<Stakeholders> GetAllStakeholders()
        {
            var session = SessionManager.GetCurrentSession();

            var stakeholder = session.Query<Stakeholders>()
                .Fetch(x => x.Hierarchy)
                .Where(x => x.LeavingDate == null || x.LeavingDate > DateTime.Today)
                .ToList();
            return stakeholder;
       }

        [Transaction]
       public IList<StkhHierarchy> GetAllHierarchy()
       {
           var session = SessionManager.GetCurrentSession();
           return session.QueryOver<StkhHierarchy>()
                         .List();
       }
    }
}
