using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class AddressQueryBuilder : Repository<StakeAddress>
    {
        public override QueryOver<StakeAddress, StakeAddress> WithRelation()
        {
            return null;
        }
    }
}