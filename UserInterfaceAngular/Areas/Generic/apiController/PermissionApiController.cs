#region references

using System;
using System.Net;
using System.Net.Http;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Transform;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

#endregion

namespace UserInterfaceAngular.app
{
    public class PermissionApiController : BaseApiController<GPermission>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<StkhHierarchy> HierarchyList()
        {
            return Session.QueryOver<StkhHierarchy>()
                          .Where(x => x.Hierarchy != "Developer" && x.Hierarchy != "External")
                          .List();
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
            return Session.QueryOver<GPermission>()
                          .Where(x => x.Role.Id == hierarchyId)
                          .Fetch(x => x.Role).Eager
                          .TransformUsing(Transformers.DistinctRootEntity)
                          .List();
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
            var hierarchy = uow.QueryOver<StkhHierarchy>()
                               .Where(x => x.Id == permissions[0].Role.Id)
                               .SingleOrDefault();

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