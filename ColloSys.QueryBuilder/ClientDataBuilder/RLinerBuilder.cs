#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class RLinerBuilder : Repository<RLiner>
    {
        public override QueryOver<RLiner, RLiner> ApplyRelations()
        {
            return QueryOver.Of<RLiner>();
        }
    }
}