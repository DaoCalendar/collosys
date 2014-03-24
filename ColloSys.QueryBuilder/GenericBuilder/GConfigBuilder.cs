﻿#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.GenericBuilder
{
    public class GConfigBuilder : Repository<GConfig>
    {
        public override QueryOver<GConfig, GConfig> WithRelation()
        {
            return QueryOver.Of<GConfig>();

        }
    }
}
