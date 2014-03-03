#region references

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using NHibernate;
using NLog;

#endregion

namespace UserInterfaceAngular.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UserActivityAttribute : AuthorizeAttribute
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public UserActivityAttribute()
        {
            Activity = ColloSysEnums.Activities.All;
            //Roles = "All";
        }

        public ColloSysEnums.Activities Activity { get; set; }

        //public virtual string Roles { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                var session = SessionManager.BindNewSession();

                using (var tx = session.BeginTransaction())
                {
                    var activity = Enum.GetName(typeof (ColloSysEnums.Activities), Activity);
                    if (httpContext == null)
                    {
                        tx.Commit();
                        return false;
                    }

                    var user = httpContext.User;
                    base.AuthorizeCore(httpContext);

                    if (!user.Identity.IsAuthenticated)
                    {
                        tx.Commit();
                        return false;
                    }

                    var privilegeLevels = string.Join(",",
                                                      GetUserRights(
                                                          httpContext.User.Identity.Name.ToString(
                                                              CultureInfo.InvariantCulture)));
                        // Call another method to get rights of the user from DB
                    if (!string.IsNullOrWhiteSpace(privilegeLevels))
                    {
                        if (Activity == ColloSysEnums.Activities.All)
                        {
                            tx.Commit();
                            return true;
                        }

                        tx.Commit();
                        return activity != null && (privilegeLevels.Contains(activity));
                    }

                    tx.Commit();
                }

                return false;

            }
            catch (Exception e)
            {
                _logger.Error("Authentication And Authorization:"+e.Message);
                 throw;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
            }
        }

        private  IEnumerable<string> GetUserRights(string userName)
        {
            var session = SessionManager.GetCurrentSession();
            try
            {
                
                var user = session.QueryOver<Users>()
                                  .Where(x => x.Username == userName)
                                  .Cacheable()
                                  .SingleOrDefault();
                //var userId = user.Role.Id;
                if (user == null)
                {
                    return new List<string>();
                }
                var userRights = session.QueryOver<GPermission>()
                                        .Where(x => x.Role.Id == user.Role.Id)
                                        .Cacheable().Skip(0).Take(500).List();
                var permissions = userRights.Where(permission => !permission.Permission.Equals(ColloSysEnums.Permissions.NoAccess)).ToList();
                return permissions.Select(gper => Enum.GetName(typeof (ColloSysEnums.Activities), gper.Activity)).ToList();
            }
            catch (HibernateException e)
            {
                _logger.Error("Getting User Rights for Login User:"+e.Message);
                return null;
            }

        }
       
    }
}