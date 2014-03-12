#region references

using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.AllocationBuilder
{
    public class AllocRelationBuilder : QueryBuilder<AllocRelation>
    {
        public override QueryOver<AllocRelation, AllocRelation> DefaultQuery()
        {
            return QueryOver.Of<AllocRelation>();
        }

        [Transaction]
        public AllocRelation OnAllocSubpolicy(AllocSubpolicy subpolicy)
        {
            return SessionManager.GetCurrentSession()
                                 .QueryOver<AllocRelation>()
                                 .Where(x => x.AllocSubpolicy.Id == subpolicy.Id)
                                 .SingleOrDefault();
        }
    }
}