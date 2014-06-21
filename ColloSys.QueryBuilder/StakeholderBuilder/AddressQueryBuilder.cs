using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.StakeholderBuilder
{
    public class AddressQueryBuilder : Repository<StkhAddress>
    {
        public override QueryOver<StkhAddress, StkhAddress> ApplyRelations()
        {
            return null;
        }
    }
}