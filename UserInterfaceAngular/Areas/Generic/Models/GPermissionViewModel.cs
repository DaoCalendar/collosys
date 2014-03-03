#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using NHibernate.Transform;

#endregion

namespace ColloSys.UserInterface.Areas.Generic.Models
{
    public class GPermissionViewModel
    {

        // ReSharper disable MemberCanBePrivate.Global
        public IEnumerable<StkhHierarchy> StakeHierarchies;
        public IEnumerable<GPermission> Permissions;
        public IEnumerable<string> Activities;
        // ReSharper restore MemberCanBePrivate.Global

        public GPermissionViewModel()
        {
            var session = SessionManager.GetCurrentSession();
            StakeHierarchies = session.QueryOver<StkhHierarchy>()
                          .Where(x => x.Hierarchy != "Developer")
                          .List();

            Permissions = session.QueryOver<GPermission>()
                                 .Fetch(x => x.Role).Eager
                                 .TransformUsing(Transformers.DistinctRootEntity)
                                 .List();

            Activities = Enum.GetNames(typeof(ColloSysEnums.Activities))
                             .Where(x => ((x.ToUpperInvariant() != "ALL")
                                          && (x.ToUpperInvariant() != "DEVELOPMENT")));
        }

    }
}