#region  References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Data.Odbc;
using System.Diagnostics;
using System.Text;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using NLog;

#endregion

namespace ColloSys.UserInterface.AuthNAuth.Membership
{
    public sealed class FnhRoleProvider : RoleProvider
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region private

        private const string EventSource = "FNHRoleProvider";
        private const string EventLog = "Application";
        private const string ExceptionMessage = "An exception occurred. Please check the Event Log.";
        private string _applicationName;

        #endregion

        #region Properties

        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        private bool WriteExceptionsToEventLog { get; set; }
        #endregion

        #region Helper Functions
        // A helper function to retrieve config values from the configuration file
        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        private void WriteToEventLog(Exception e, string action)
        {
            var log = new EventLog { Source = EventSource, Log = EventLog };

            var message = ExceptionMessage + "\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e;

            log.WriteEntry(message);
        }
        #endregion

        #region Private Methods
        //get a role by name
        private StkhHierarchy GetRole(string rolename)
        {
            StkhHierarchy role = null;
            var session = SessionManager.GetCurrentSession();
            try
            {
                role = session.QueryOver<StkhHierarchy>()
                              .Where(x => x.Designation == rolename)
                              .SingleOrDefault<StkhHierarchy>();

                //just to lazy init the collection, otherwise get the error 
                //NHibernate.LazyInitializationException: failed to lazily initialize a collection, no session or session was closed
                //Iesi.Collections.Generic.ISet<Users> us = new HashedSet<Users>(role.UsersInRole);

            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetRole");
                else
                    throw;
            }

            return role;
        }

        #endregion

        #region Public Methods
        //initializes the FNH role provider
        public override void Initialize(string name, NameValueCollection config)
        {
            // Initialize values from web.config.

            if (config == null)
                throw new ArgumentNullException("config");

            if (name.Length == 0)
                name = "FNHRoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Sample Fluent Nhibernate Role provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            _applicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            WriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

        }

        //adds a user collection toa roles collection
        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            Users usr = null;
            foreach (string rolename in rolenames)
            {
                if (!RoleExists(rolename))
                    throw new ProviderException(String.Format("Role name {0} not found.", rolename));
            }

            foreach (string username in usernames)
            {
                if (username.Contains(","))
                    throw new ArgumentException(String.Format("User names {0} cannot contain commas.", username));
                //is user not exiting //throw exception

                foreach (string rolename in rolenames)
                {
                    if (IsUserInRole(username, rolename))
                        throw new ProviderException(String.Format("User {0} is already in role {1}.", username, rolename));
                }
            }

            var session = SessionManager.GetCurrentSession();
            try
            {
                foreach (var username in usernames)
                {
                    foreach (var rolename in rolenames)
                    {
                        //get the user 
                        var username1 = username;
                        usr = session.QueryOver<Users>()
                                     .Where(x => x.Username == username1)
                                     .SingleOrDefault<Users>();

                        if (usr == null) continue;
                        //get the role first from db
                        var rolename1 = rolename;
                        var role = session.QueryOver<StkhHierarchy>()
                                          .Where(x => x.Designation == rolename1)
                                          .SingleOrDefault<StkhHierarchy>();
                        usr.AddRole(role);
                    }
                    session.SaveOrUpdate(usr);
                }

            }
            catch (Exception e)
            {
                if (!WriteExceptionsToEventLog)
                {
                    throw;
                }

                _logger.Error(e.Message);
                WriteToEventLog(e, "AddUsersToRoles");
            }
        }

        //create  a new role with a given name
        public override void CreateRole(string rolename)
        {
            if (rolename.Contains(","))
                throw new ArgumentException("Role names cannot contain commas.");

            if (RoleExists(rolename))
                throw new ProviderException("Role name already exists.");
            var session = SessionManager.GetCurrentSession();
            try
            {
                var role = new StkhHierarchy { Designation = rolename };
                session.SaveOrUpdate(role);
            }
            catch (OdbcException e)
            {
                if (!WriteExceptionsToEventLog)
                {
                    throw;
                }
                _logger.Error(e.Message);
                WriteToEventLog(e, "CreateRole");
            }
        }

        //delete a role with given name
        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            if (!RoleExists(rolename))
                throw new ProviderException("Role does not exist.");

            if (throwOnPopulatedRole && GetUsersInRole(rolename).Length > 0)
                throw new ProviderException("Cannot delete a populated role.");

            try
            {
                StkhHierarchy role = GetRole(rolename);
                SessionManager.GetCurrentSession().Delete(role);
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteRole");
                    _logger.Error(e.Message);
                    return false;
                }
                throw;
            }

            return false;
        }

        //get an array of all the roles
        public override string[] GetAllRoles()
        {
            var sb = new StringBuilder();
            var session = SessionManager.GetCurrentSession();
            try
            {
                var allroles = session.QueryOver<StkhHierarchy>()
                                      .List<StkhHierarchy>();

                foreach (StkhHierarchy r in allroles)
                {
                    sb.Append(r.Designation + ",");
                }
            }
            catch (Exception e)
            {
                if (!WriteExceptionsToEventLog)
                {
                    throw;
                }
                _logger.Error(e.Message);
                WriteToEventLog(e, "GetAllRoles");
            }
            if (sb.Length > 0)
            {
                // Remove trailing comma.
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString().Split(',');
            }

            return new string[0];
        }

        //Get roles for a user by username

        #region  For getting the roles for a user by username

        //public override string[] GetRolesForUser(string username)
        //{
        //    Users usr = null;
        //    IList<StakeHierarchy> usrroles = new List<StakeHierarchy>();
        //    StringBuilder sb = new StringBuilder();
        //    var session = SessionManager.GetCurrentSession();
        //    try
        //    {
        //        usr = session.QueryOver<Users>()
        //                     .Where(x => x.Username == username && x.ApplicationName == this.ApplicationName)
        //                     .SingleOrDefault<Users>();
        //        if (usr != null)
        //        {
        //            usrroles.Add(usr.Role);
        //            foreach (StakeHierarchy r in usrroles)
        //            {
        //                sb.Append(r.Designation + ",");
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (WriteExceptionsToEventLog)
        //            WriteToEventLog(e, "GetRolesForUser");
        //        else
        //            throw e;
        //    }

        //    if (sb.Length > 0)
        //    {
        //        // Remove trailing comma.
        //        sb.Remove(sb.Length - 1, 1);
        //        return sb.ToString().Split(',');
        //    }

        //    return new string[0];
        //}

        #endregion


        public override string[] GetRolesForUser(string username)
        {
            var permissions = new List<GPermission>();
            IList<StkhHierarchy> usrroles = new List<StkhHierarchy>();
            var sb = new StringBuilder();
            var session = SessionManager.GetCurrentSession();
            try
            {
                using (var tx = session.BeginTransaction())
                {
                    var usr = session.QueryOver<Users>()
                                     .Where(x => x.Username == username)
                                     .Cacheable()
                                     .SingleOrDefault<Users>();

                    var activities = session.QueryOver<GPermission>()
                                            .Where(x => x.Role.Id == usr.Role.Id)
                                            .Cacheable()
                                            .Skip(0).Take(500)
                                            .List<GPermission>();

                    permissions.AddRange(
                        activities.Where(permission => !permission.Permission.Equals(ColloSysEnums.Permissions.NoAccess)));

                    if (usr != null)
                    {
                        usrroles.Add(usr.Role);
                        foreach (
                            var activity in
                                permissions.Select(
                                    permission => Enum.GetName(typeof(ColloSysEnums.Activities), permission.Activity)))
                        {
                            sb.Append("All" + "," + activity + ",");
                        }
                    }

                    tx.Commit();
                }
            }
            catch (Exception e)
            {
                if (!WriteExceptionsToEventLog)
                    throw;
                _logger.Error(e.Message);
                WriteToEventLog(e, "GetRolesForUser");
            }

            if (sb.Length > 0)
            {
                // Remove trailing comma.
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString().Split(',');
            }

            return new string[0];
        }
        //Get users in a givenrolename
        public override string[] GetUsersInRole(string rolename)
        {
            var sb = new StringBuilder();
            var session = SessionManager.GetCurrentSession();
            try
            {
                var role = session.QueryOver<StkhHierarchy>()
                                  .Where(x => x.Designation == rolename)
                                  .SingleOrDefault<StkhHierarchy>();
                var usrs = role.UsersInRole;

                foreach (var u in usrs)
                {
                    sb.Append(u.Username + ",");
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    _logger.Error(e.Message);
                    WriteToEventLog(e, "GetUsersInRole");

                }
                else
                    throw;
            }
            if (sb.Length > 0)
            {
                // Remove trailing comma.
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString().Split(',');
            }

            return new string[0];
        }

        //determine is a user has a given role
        public override bool IsUserInRole(string username, string rolename)
        {
            var userIsInRole = false;
            var usrroles = new List<StkhHierarchy>();
            var session = SessionManager.GetCurrentSession();
            try
            {
                var usr = session.QueryOver<Users>()
                                 .Where(x => x.Username == username)
                                 .SingleOrDefault<Users>();
                if ((usr != null))
                {
                    usrroles.Add(usr.Role);
                    if (usrroles.Any(r => r.Designation.Equals(rolename)))
                    {
                        userIsInRole = true;
                    }
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    _logger.Error(e.Message);
                    WriteToEventLog(e, "IsUserInRole");

                }
                else
                    throw;
            }
            return userIsInRole;
        }

        //remeove users from roles
        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            foreach (string rolename in rolenames)
            {
                if (!RoleExists(rolename))
                    throw new ProviderException(String.Format("Role name {0} not found.", rolename));
            }

            foreach (string username in usernames)
            {
                foreach (string rolename in rolenames)
                {
                    if (!IsUserInRole(username, rolename))
                        throw new ProviderException(String.Format("User {0} is not in role {1}.", username, rolename));
                }
            }

            //get user , get his roles , the remove the role and save   
            var session = SessionManager.GetCurrentSession();
            try
            {
                foreach (var username in usernames)
                {
                    var username1 = username;
                    var usr = session.QueryOver<Users>()
                                     .Where(x => x.Username == username1)
                                     .SingleOrDefault<Users>();

                    var rolestodelete = new List<StkhHierarchy>();
                    foreach (var rolename in rolenames)
                    {
                        IList<StkhHierarchy> roles = new List<StkhHierarchy>();
                        roles.Add(usr.Role);
                        rolestodelete.AddRange(roles.Where(r => r.Designation.Equals(rolename)));
                    }
                    foreach (var rd in rolestodelete)
                        usr.RemoveRole(rd);

                    SessionManager.GetCurrentSession().SaveOrUpdate(usr);
                }
            }
            catch (OdbcException e)
            {
                if (!WriteExceptionsToEventLog)
                    throw;
                _logger.Error(e.Message);
                WriteToEventLog(e, "RemoveUsersFromRoles");
            }
        }

        //boolen to check if a role exists given a role name
        public override bool RoleExists(string rolename)
        {
            var exists = false;

            var session = SessionManager.GetCurrentSession();
            try
            {
                var role = session.QueryOver<StkhHierarchy>()
                                  .Where(x => x.Designation == rolename)
                                  .SingleOrDefault<StkhHierarchy>();
                if (role != null)
                    exists = true;

            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    _logger.Error(e.Message);
                    WriteToEventLog(e, "RoleExists");

                }
                else
                    throw;
            }
            return exists;
        }

        //find users that beloeng to a particular role , given a username, Note : does not do a LIke search
        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            var sb = new StringBuilder();
            var session = SessionManager.GetCurrentSession();
            try
            {
                var role = session.QueryOver<StkhHierarchy>()
                                  .Where(x => x.Designation == rolename)
                                  .SingleOrDefault<StkhHierarchy>();

                var users = role.UsersInRole;
                if (users != null)
                {
                    foreach (var u in users.Where(u => String.Compare(u.Username, usernameToMatch, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        sb.Append(u.Username + ",");
                    }
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    _logger.Error(e.Message);
                    WriteToEventLog(e, "FindUsersInRole");

                }
                else
                    throw;
            }

            if (sb.Length > 0)
            {
                // Remove trailing comma.
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString().Split(',');
            }
            return new string[0];

        }

        #endregion
    }
}
