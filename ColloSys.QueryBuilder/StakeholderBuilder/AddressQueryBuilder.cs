using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class AddressQueryBuilder : Repository<StakeAddress>
    {
        public override QueryOver<StakeAddress, StakeAddress> ApplyRelations()
        {
            return null;
        }
    }
}