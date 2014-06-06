#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class CacsActivityBuilder : Repository<CacsActivity>
    {
        public override QueryOver<CacsActivity, CacsActivity> ApplyRelations()
        {
            return QueryOver.Of<CacsActivity>();
        }

        [Transaction]
        public IEnumerable<CacsActivity> DataOnFileSchedular(FileScheduler fileScheduler)
        {
            return SessionManager.GetCurrentSession().QueryOver<CacsActivity>()
                              .Where(x => x.FileScheduler.Id == fileScheduler.Id)
                              .And(x => x.ConsiderInAllocation)
                              .List();
        }
    }
}
