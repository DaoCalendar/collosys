#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.GenericBuilder
{
    public class GConfigBuilder : QueryBuilder<GConfig>
    {
        public override QueryOver<GConfig, GConfig> DefaultQuery()
        {
            return QueryOver.Of<GConfig>();

        }
    }
}
