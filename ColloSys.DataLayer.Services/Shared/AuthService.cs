using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;

namespace ColloSys.DataLayer.Services.Shared
{
    public static class AuthService
    {
        public static string CurrentUser
        {
            get
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.User.Identity.IsAuthenticated))
                {
                    return HttpContext.Current.User.Identity.Name;
                }

                var windowsIdentity = WindowsIdentity.GetCurrent();
                var name = windowsIdentity != null ? windowsIdentity.Name : @"scb\anonymous";
                var stop = name.IndexOf("\\", StringComparison.Ordinal);
                return (stop > -1) ? name.Substring(stop + 1, name.Length - stop - 1) : name;
            }
        }

        public static IEnumerable<GPermission> GetPremissionsForCurrentUser()
        {
            var session = SessionManager.GetCurrentSession();
            var currUserInfo = session.QueryOver<GUsers>()
                                      .Where(x => x.Username == CurrentUser)
                                      .Select(x => x.Role)
                                      .SingleOrDefault<StkhHierarchy>();
            var permisions = session.QueryOver<GPermission>()
                                    .Where(x => x.Role.Id == currUserInfo.Id)
                                    .List<GPermission>();
            return permisions;
        }

        public static IEnumerable<GPermission> GetPremissionsForCurrentUser(string username)
        {
            var session = SessionManager.GetCurrentSession();
            var currUserInfo = session.QueryOver<GUsers>()
                                      .Where(x => x.Username == username)
                                      .Select(x => x.Role)
                                      .SingleOrDefault<StkhHierarchy>();
            var permisions = session.QueryOver<GPermission>()
                                    .Where(x => x.Role.Id == currUserInfo.Id)
                                    .List<GPermission>();
            return permisions;
        }
    }
}
