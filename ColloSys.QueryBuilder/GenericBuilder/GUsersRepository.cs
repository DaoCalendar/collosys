using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

namespace ColloSys.QueryBuilder.GenericBuilder
{
    public class GUsersRepository : Repository<Users>
    {
        public override QueryOver<Users, Users> ApplyRelations()
        {
            return QueryOver.Of<Users>();
        }
    }
}
