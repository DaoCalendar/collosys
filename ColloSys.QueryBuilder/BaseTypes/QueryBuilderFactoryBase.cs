using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

namespace ColloSys.QueryBuilder.BaseTypes
{
    public abstract class QueryBuilderFactoryBase
    {
        public abstract IQueryBuilder<T> BuilderFor<T>(TypeOf type) where T:Entity;
    }

    public enum TypeOf
    {
        Stakeholder,
        StkhWorking,
        StkhPayment,
        StkhRegistration,
        StkhHierarchy,
    }

}
