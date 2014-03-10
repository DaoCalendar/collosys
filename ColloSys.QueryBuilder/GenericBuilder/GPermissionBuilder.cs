#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.Transform;

#endregion

namespace ColloSys.QueryBuilder.GenericBuilder
{
    public class GPermissionBuilder : QueryBuilder<GPermission>
    {
        [Transaction]
        public IEnumerable<GPermission> OnHierarchyId(Guid hierarchyId)
        {
            return SessionManager.GetCurrentSession().QueryOver<GPermission>()
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
}