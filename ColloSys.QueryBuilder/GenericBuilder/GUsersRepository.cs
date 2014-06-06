using ColloSys.DataLayer.Generic;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.GenericBuilder
{
    public class GUsersRepository : Repository<GUsers>
    {
        public override QueryOver<GUsers, GUsers> ApplyRelations()
        {
            return QueryOver.Of<GUsers>().Fetch(x => x.Role).Eager;
        }
    }
}
