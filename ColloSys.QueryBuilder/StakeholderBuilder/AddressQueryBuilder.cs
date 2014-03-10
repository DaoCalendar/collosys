using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class AddressQueryBuilder : QueryBuilder<StakeAddress>
    {
        public override QueryOver<StakeAddress, StakeAddress> DefaultQuery()
        {
            return null;
        }
    }
}