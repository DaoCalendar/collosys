using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.GenericBuilder
{
    public class GUsersRepository : Repository<GConfig>
    {
        public override QueryOver<GConfig, GConfig> WithRelation()
        {
            return QueryOver.Of<GConfig>();
        }
    }
}
