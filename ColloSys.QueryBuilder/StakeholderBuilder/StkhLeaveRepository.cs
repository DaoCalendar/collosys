#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Linq;

#endregion

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class StkhLeaveRepository : Repository<StkhLeave>
    {
        [Transaction]
        public IList<Stakeholders> GetDelegatedToMe(Guid stakeholderId, DateTime todayDate)
        {
            var session = SessionManager.GetCurrentSession();
            return session.Query<StkhLeave>()
                .Where(x => x.DelegatedTo.Id == stakeholderId
                    && (x.FromDate <= todayDate && x.ToDate >= todayDate))
                .Select(x => x.DelegatedTo)
                .ToList();
        }
    }
}
