#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.GenericBuilder
{
    public class ProductConfigBuilder : Repository<ProductConfig>
    {
        [Transaction]

        public IEnumerable<ScbEnums.Products> GetProducts()
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<ProductConfig>()
                          .Select(x => x.Product).List<ScbEnums.Products>();
        }

        public override QueryOver<ProductConfig, ProductConfig> WithRelation()
        {
            return QueryOver.Of<ProductConfig>();
        }
    }
}