#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class RLinerBuilder : QueryBuilder<RLiner>
    {
        public override QueryOver<RLiner, RLiner> DefaultQuery()
        {
            return QueryOver.Of<RLiner>();
        }
    }
}