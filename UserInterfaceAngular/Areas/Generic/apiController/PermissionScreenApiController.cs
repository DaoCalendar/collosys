#region references

using System;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Transform;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

#endregion

namespace ColloSys.UserInterface.Areas.Generic.apiController
{
    public class PermissionScreenApiController : BaseApiController<GPermission>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly static HierarchyQueryBuilder HierarchyQuery = new HierarchyQueryBuilder();
        private readonly static GPermissionBuilder GPermissionBuilder = new GPermissionBuilder();

        [HttpGet]
        [HttpTransaction]
        public PermisionData GetWholeData()
        {
            PermisionData permision = new PermisionData();

            permision.HierarchyData = HierarchyQuery.ExceptDeveloperExternal().ToList();
            //Session.QueryOver<StkhHierarchy>()
            //       .Where(x => x.Hierarchy != "Developer" && x.Hierarchy != "External")
            //       .List();
            permision.ActivityData = Enum.GetNames(typeof(ColloSysEnums.Activities))
                     .Where(x => ((x.ToUpperInvariant() != "ALL")
                                    && (x.ToUpperInvariant() != "DEVELOPMENT")));

            var query = GPermissionBuilder.ApplyRelations();
            permision.PermissionData = GPermissionBuilder.Execute(query).ToList();
            return permision;

        }

        private IEnumerable<GPermission> RetrievePermissions(Guid hierarchyId)
        {
            return GPermissionBuilder.OnHierarchyId(hierarchyId);
        }

        #region Post

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public IEnumerable<GPermission> SavePermissions(IList<GPermission> permissions)
        {
            foreach (var gPermission in permissions)
            {
                GPermissionBuilder.Save(gPermission);
            }
            var query = GPermissionBuilder.ApplyRelations();
            var data = GPermissionBuilder.Execute(query).ToList();
            return data;
        }

        private static void SaveOrUpdate(IList<GPermission> permissions)
        {
            if (permissions.Count == 0)
            {
                return;
            }

            StkhHierarchy hierarchy = null;
            foreach (var gPermission in permissions)
            {
                GPermission permission = gPermission;
                GPermission permission1 = gPermission;
                hierarchy =
                    HierarchyQuery.FilterBy(
                        x => permission != null && x.Hierarchy == permission.Role.Hierarchy &&
                             permission1 != null && x.Designation == permission1.Role.Designation).FirstOrDefault();
                if (gPermission.Id == Guid.Empty)
                {

                    gPermission.Role = hierarchy;
                    hierarchy.GPermissions.Add(gPermission);
                }
                else
                {
                    var resPerm = gPermission;
                    if (hierarchy != null)
                    {
                        var perm = hierarchy.GPermissions.SingleOrDefault(x => x.Id == resPerm.Id);
                        if (perm == null)
                        {
                            hierarchy.GPermissions.Add(resPerm);
                        }
                        else
                        {
                            perm.EscalationDays = gPermission.EscalationDays;
                            perm.Permission = gPermission.Permission;
                        }
                    }
                }

            }
            HierarchyQuery.Save(hierarchy);
        }
        #endregion
    }
}
