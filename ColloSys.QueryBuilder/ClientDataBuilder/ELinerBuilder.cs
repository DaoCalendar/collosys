#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class ELinerBuilder : QueryBuilder<ELiner>
    {
        public override QueryOver<ELiner, ELiner> DefaultQuery()
        {
            return QueryOver.Of<ELiner>();
        }
    }
}