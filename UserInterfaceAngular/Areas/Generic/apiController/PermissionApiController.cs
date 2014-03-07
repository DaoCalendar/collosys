#region references

using System;
using System.Net;
using System.Net.Http;
using ColloSys.DataLayer.Components;
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

//StkhHierarchy calls changed
namespace UserInterfaceAngular.app
{
    public class PermissionApiController : BaseApiController<GPermission>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static readonly HierarchyQueryBuilder hierarchyQuery=new HierarchyQueryBuilder();
        private static readonly GPermissionBuilder PermissionBuilder=new GPermissionBuilder();

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<StkhHierarchy> HierarchyList()
        {
            return hierarchyQuery.ExceptDeveloperExternal();
        }

        [HttpGet]
        public IEnumerable<string> ActivityList()
        {
            return Enum.GetNames(typeof(ColloSysEnums.Activities))
                       .Where(x => ((x.ToUpperInvariant() != "ALL")
                                    && (x.ToUpperInvariant() != "DEVELOPMENT")));
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<GPermission> PermissionList(Guid hierarchyId)
        {
            return RetrievePermissions(hierarchyId);
        }

        private IEnumerable<GPermission> RetrievePermissions(Guid hierarchyId)
        {
            return PermissionBuilder.OnHierarchyId(hierarchyId);
        }

        #region Post

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public HttpResponseMessage SavePermissions(IList<GPermission> permissions)
        {
            try
            {
                SaveOrUpdate(permissions);
                var perms = RetrievePermissions(permissions[0].Role.Id);
                return Request.CreateResponse(HttpStatusCode.Created, perms);
            }

            catch (Exception exception)
            {
                _log.ErrorException(exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.BadRequest, exception);
            }
        }

        private static void SaveOrUpdate(IList<GPermission> permissions)
        {
            if (permissions.Count == 0)
            {
                return;
            }

            var uow = SessionManager.GetCurrentSession();
            var hierarchy = hierarchyQuery.GetOnExpression(x => x.Id == permissions[0].Role.Id).FirstOrDefault();
                               // uow.QueryOver<StkhHierarchy>()
                               //.Where(x => x.Id == permissions[0].Role.Id)
                               //.SingleOrDefault();

            foreach (var gPermission in permissions)
            {
                if (gPermission.Id == Guid.Empty)
                {
                    gPermission.Role = hierarchy;
                    hierarchy.GPermissions.Add(gPermission);
                }
                else
                {
                    var resPerm = gPermission;
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

            // save all chagnes
            uow.SaveOrUpdate(hierarchy);
        }

        #endregion

    }
}