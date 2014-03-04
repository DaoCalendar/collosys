using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.StakeholderBuilder;

namespace ColloSys.QueryBuilder.BaseTypes
{
    public class QueryBuilderFactory:QueryBuilderFactoryBase
    {
        public override IQueryBuilder<T> BuilderFor<T>(TypeOf type)
        {
            switch (type)
            {
                case TypeOf.Stakeholder:
                    return (IQueryBuilder<T>)new StakeQueryBuilder();
                case TypeOf.StkhWorking:
                    return (IQueryBuilder<T>) new StakeWorkingQueryBuilder();
                    break;
                case TypeOf.StkhPayment:
                    break;
                case TypeOf.StkhRegistration:
                    break;
                case TypeOf.StkhHierarchy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
            return null;
        }
    }
}