#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class CacsActivityBuilder : QueryBuilder<CacsActivity>
    {
        public override QueryOver<CacsActivity, CacsActivity> DefaultQuery()
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
