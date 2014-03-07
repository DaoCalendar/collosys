using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using System.Linq;
using NHibernate.Transform;

namespace ColloSys.QueryBuilder.GenericBuilder
{
    public class GConfigBuilder : QueryBuilder<GConfig>
    {
        public override QueryOver<GConfig, GConfig> DefaultQuery()
        {
            return QueryOver.Of<GConfig>();

        }
    }

    public class GKeyValueBuilder:QueryBuilder<GKeyValue>
    {
        public override QueryOver<GKeyValue, GKeyValue> DefaultQuery()
        {
            return QueryOver.Of<GKeyValue>();
        }
    }

    public class GPermissionBuilder:QueryBuilder<GPermission>
    {
        [Transaction]
        public IEnumerable<GPermission> OnHierarchyId(Guid hierarchyId)
        {
            SessionManager.GetCurrentSession().QueryOver<GPermission>()
                          .Where(x => x.Role.Id == hierarchyId)
                          .Fetch(x => x.Role).Eager
                          .TransformUsing(Transformers.DistinctRootEntity)
                          .List();
        }

        public override QueryOver<GPermission, GPermission> DefaultQuery()
        {
            return QueryOver.Of<GPermission>();
        }
    }

    public class GPincodeBuilder:QueryBuilder<GPincode>
    {
        public override QueryOver<GPincode, GPincode> DefaultQuery()
        {
            return QueryOver.Of<GPincode>();
        }
    }

    public class GReportBuilder : QueryBuilder<GReports>
    {
        public override QueryOver<GReports, GReports> DefaultQuery()
        {
            return QueryOver.Of<GReports>();
        }
    }

    public class ProductConfigBuilder:QueryBuilder<ProductConfig>
    {
        [Transaction]

        public IEnumerable<ScbEnums.Products> GetProducts()
        {
            var session = SessionManager.GetCurrentSession();
            return session.QueryOver<ProductConfig>()
                          .Select(x => x.Product).List<ScbEnums.Products>();
        }

        public override QueryOver<ProductConfig, ProductConfig> DefaultQuery()
        {
            return QueryOver.Of<ProductConfig>();
        }
    }
}
