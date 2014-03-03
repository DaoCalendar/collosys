using System;
using System.Collections;
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

namespace ColloSys.UserInterface.Areas.Generic.apiController
{
    public class PermissionScreenApiController : BaseApiController<GPermission>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [HttpTransaction]
        public PermisionData GetWholeData()
        {
            PermisionData permision = new PermisionData();
            permision.HierarchyData = Session.QueryOver<StkhHierarchy>()
                                       .Where(x => x.Hierarchy != "Developer" && x.Hierarchy != "External")
                                       .List();
            permision.ActivityData = Enum.GetNames(typeof(ColloSysEnums.Activities))
                     .Where(x => ((x.ToUpperInvariant() != "ALL")
                                    && (x.ToUpperInvariant() != "DEVELOPMENT")));

            permision.PermissionData = Session.QueryOver<GPermission>().Fetch(x=>x.Role).Eager
                                        .List();

            return permision;

        }

        //[HttpGet]
        //[HttpTransaction]
        //public IEnumerable<StkhHierarchy> HierarchyList()
        //{
        //    return Session.QueryOver<StkhHierarchy>()
        //                  .Where(x => x.Hierarchy != "Developer" && x.Hierarchy != "External")
        //                  .List();
        //}

        //[HttpGet]
        //public IEnumerable<string> ActivityList()
        //{
        //    return Enum.GetNames(typeof(ColloSysEnums.Activities))
        //               .Where(x => ((x.ToUpperInvariant() != "ALL")
        //                            && (x.ToUpperInvariant() != "DEVELOPMENT")));
        //}

        //[HttpGet]
        //[HttpTransaction]
        //public IEnumerable<GPermission> PermissionList(Guid hierarchyId)
        //{
        //    return RetrievePermissions(hierarchyId);
        //}

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
        public IEnumerable<GPermission> SavePermissions(IList<GPermission> permissions)
        {
            var uow = SessionManager.GetCurrentSession();
            
            foreach (var gPermission in permissions)
            {
                uow.SaveOrUpdate(gPermission);
            }
            var data=uow.QueryOver<GPermission>().Fetch(x=>x.Role).Eager.List();
            return data;
        }

        private static void SaveOrUpdate(IList<GPermission> permissions)
        {
            if (permissions.Count == 0)
            {
                return;
            }

            var uow = SessionManager.GetCurrentSession();
            //var hierarchy = uow.QueryOver<StkhHierarchy>()
            //                   .Where(x => x.Id == permissions[0].Role.Id)
            //                   .SingleOrDefault();
            StkhHierarchy hierarchy = null;
            foreach (var gPermission in permissions)
            {
                GPermission permission = gPermission;
                GPermission permission1 = gPermission;
                hierarchy = uow.QueryOver<StkhHierarchy>().Where(x => permission != null && x.Hierarchy == permission.Role.Hierarchy)
                                   .And(x => permission1 != null && x.Designation == permission1.Role.Designation).SingleOrDefault();
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

            // save all chagnes
            uow.SaveOrUpdate(hierarchy);
        }

        #endregion
    }
}
