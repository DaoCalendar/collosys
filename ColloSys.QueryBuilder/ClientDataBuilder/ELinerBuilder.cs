#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class ELinerBuilder : Repository<ELiner>
    {
        public override QueryOver<ELiner, ELiner> ApplyRelations()
        {
            return QueryOver.Of<ELiner>();
        }
    }
}