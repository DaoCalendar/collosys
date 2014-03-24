#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class CWriteoffBuilder : Repository<CWriteoff>
    {
        public override QueryOver<CWriteoff, CWriteoff> ApplyRelations()
        {
            return QueryOver.Of<CWriteoff>();
        }
    }
}