#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class RWriteoffBuilder : Repository<RWriteoff>
    {
        public override QueryOver<RWriteoff, RWriteoff> WithRelation()
        {
            return QueryOver.Of<RWriteoff>();
        }
    }
}