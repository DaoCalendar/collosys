#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class CLinerBuilder : QueryBuilder<CLiner>
    {
        public override QueryOver<CLiner, CLiner> DefaultQuery()
        {
            return QueryOver.Of<CLiner>();
        }
    }
}