#region references

using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<string> GetProductsString()
        {
            var list = GetProducts();
            return list.Select(productse => productse.ToString()).ToList();
        }

        public override QueryOver<ProductConfig, ProductConfig> ApplyRelations()
        {
            return QueryOver.Of<ProductConfig>();
        }
    }
}