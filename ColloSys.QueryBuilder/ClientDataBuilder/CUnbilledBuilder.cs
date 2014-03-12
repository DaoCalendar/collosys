#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class CUnbilledBuilder : QueryBuilder<CUnbilled>
    {
        public override QueryOver<CUnbilled, CUnbilled> DefaultQuery()
        {
            return QueryOver.Of<CUnbilled>();
        }
    }
}