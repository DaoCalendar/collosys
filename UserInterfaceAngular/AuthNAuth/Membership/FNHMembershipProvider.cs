#region  references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Data.Odbc;
using System.Configuration;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using NLog;

#endregion

namespace ColloSys.UserInterface.AuthNAuth.Membership
{
    public sealed class FnhMembershipProvider : MembershipProvider
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Private
        // Global connection string, generated password length, generic exception message, event log info.
        private const int NewPasswordLength = 8;
        private const string EventSource = "FNHMembershipProvider";
        private const string EventLog = "Application";
        private const string ExceptionMessage = "An exception occurred. Please check the Event Log.";

        private string _applicationName;
        private bool _enablePasswordReset;
        private bool _enablePasswordRetrieval;
        private bool _requiresQuestionAndAnswer;
        private bool _requiresUniqueEmail;
        private int _maxInvalidPasswordAttempts;
        private int _passwordAttemptWindow;
        private MembershipPasswordFormat _passwordFormat;
        // Used when determining encryption key values.
        private MachineKeySection _machineKey;
        private int _minRequiredNonAlphanumericCharacters;
        private int _minRequiredPasswordLength;
        private string _passwordStrengthRegularExpression;

        #endregion

        public FnhMembershipProvider()
        {
            _applicationName = "ColloSys";
        }

        #region Public Propeties

        public override string Name
        {
            get
            {
                return "FNHProvider";
            }
        }
        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get { return _enablePasswordReset; }
        }


        public override bool EnablePasswordRetrieval
        {
            get { return _enablePasswordRetrieval; }
        }


        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }


        public override bool RequiresUniqueEmail
        {
            get { return _requiresUniqueEmail; }
        }


        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }


        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
        }


        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
            // set { _passwordFormat = value; }
        }


        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
        }

        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        private bool WriteExceptionsToEventLog { get; set; }

        #endregion

        #region Helper functions
        // A Function to retrieve config values from the configuration file
        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        //Fn to create a Membership user from a Users class
        private MembershipUser GetMembershipUserFromUser(Users usr)
        {
            var u = new MembershipUser(Name,
                                                  usr.Username,
                                                  usr.Id,
                                                  usr.Email,
                                                  usr.PasswordQuestion,
                                                  usr.Comment,
                                                  usr.IsApproved,
                                                  usr.IsLockedOut,
                                                  usr.CreatedOn,
                                                  usr.LastLoginDate,
                                                  usr.LastActivityDate,
                                                  usr.LastPasswordChangedDate,
                                                  usr.LastLockedOutDate
                                                  );

            return u;
        }

        //Fn that performs the checks and updates associated with password failure tracking
        private void UpdateFailureCount(string username, string failureType)
        {
            var windowStart = new DateTime();
            var failureCount = 0;
            try
            {
                Users usr = GetUserByUsername(username);
                if (usr == null)
                    return;

                if (failureType == "password")
                {
                    failureCount = usr.FailedPasswordAttemptCount;
                    windowStart = usr.FailedPasswordAttemptWindowStart;
                }

                if (failureType == "passwordAnswer")
                {
                    failureCount = usr.FailedPasswordAnswerAttemptCount;
                    windowStart = usr.FailedPasswordAnswerAttemptWindowStart;
                }

                var windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

                if (failureCount == 0 || DateTime.Now > windowEnd)
                {
                    // First password failure or outside of PasswordAttemptWindow. 
                    // Start a new password failure count from 1 and a new window starting now.

                    if (failureType == "password")
                    {
                        usr.FailedPasswordAttemptCount = 1;
                        usr.FailedPasswordAttemptWindowStart = DateTime.Now;
                    }

                    if (failureType == "passwordAnswer")
                    {
                        usr.FailedPasswordAnswerAttemptCount = 1;
                        usr.FailedPasswordAnswerAttemptWindowStart = DateTime.Now;
                    }
                    SessionManager.GetCurrentSession().SaveOrUpdate(usr);
                }
                else
                {
                    if (failureCount++ == MaxInvalidPasswordAttempts)
                    {
                        // Password attempts have exceeded the failure threshold. Lock out
                        // the user.
                        usr.IsLockedOut = true;
                        usr.LastLockedOutDate = DateTime.Now;
                        SessionManager.GetCurrentSession().SaveOrUpdate(usr);
                    }
                    else
                    {
                        // Password attempts have not exceeded the failure threshold. Update
                        // the failure counts. Leave the window the same.

                        if (failureType == "password")
                            usr.FailedPasswordAttemptCount = failureCount;

                        if (failureType == "passwordAnswer")
                            usr.FailedPasswordAnswerAttemptCount = failureCount;

                        SessionManager.GetCurrentSession().SaveOrUpdate(usr);
                    }
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateFailureCount");
                    throw new ProviderException("Unable to update failure count and window start." + ExceptionMessage);
                }
                throw;
            }
        }

        //CheckPassword: Compares password values based on the MembershipPasswordFormat.
        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
            }

            if (pass1 == pass2)
            {
                return true;
            }

            _logger.Error("Password Not Matched");

            return false;
        }

        //EncodePassword:Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        private string EncodePassword(string password)
        {
            string encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                      Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    var hash = new HMACSHA1 { Key = HexToByte(_machineKey.ValidationKey) };
                    encodedPassword =
                      Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }
            return encodedPassword;
        }
        public string GetEncodePassword(string password)
        {
            return Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
        }

        // UnEncodePassword :Decrypts or leaves the password clear based on the PasswordFormat.
        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        //   Converts a hexadecimal string to a byte array. Used to convert encryption key values from the configuration.    
        private byte[] HexToByte(string hexString)
        {
            var returnBytes = new byte[hexString.Length / 2];
            for (var i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        // WriteToEventLog
        // A helper function that writes exception detail to the event log. Exceptions
        // are written to the event log as a security measure to avoid private database
        // details from being returned to the browser. If a method does not return a status
        // or boolean indicating the action succeeded or failed, a generic exception is also 
        // thrown by the caller.

        private void WriteToEventLog(Exception e, string action)
        {
            var log = new EventLog { Source = EventSource, Log = EventLog };
            var message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e;
            _logger.Error(message);
            log.WriteEntry(message);
        }

        #endregion

        #region Private Methods

        //single fn to get a membership user by key or username
        private MembershipUser GetMembershipUserByKeyOrUser(bool isKeySupplied, string username, object providerUserKey, bool userIsOnline)
        {
            MembershipUser u = null;
            try
            {
                var session = SessionManager.GetCurrentSession();
                Users usr;
                if (isKeySupplied)
                {
                    usr = session.QueryOver<Users>().Where(x => x.Id == (Guid)providerUserKey).Cacheable().SingleOrDefault();
                }
                else
                {
                    usr = session.QueryOver<Users>()
                                 .Where(x => x.Username == username && x.ApplicationName == ApplicationName).Cacheable()
                                 .SingleOrDefault();
                }

                if (usr != null)
                {
                    u = GetMembershipUserFromUser(usr);

                    if (userIsOnline)
                    {
                        usr.LastActivityDate = DateTime.Now;
                        SessionManager.GetCurrentSession().SaveOrUpdate(usr);
                    }
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUser(Object, Boolean)");
                throw new ProviderException(ExceptionMessage);
            }
            return u;
        }

        private Users GetUserByUsername(string username)
        {
            Users usr;

            try
            {
                var session = SessionManager.GetCurrentSession();
                usr = session.QueryOver<Users>()
                      .Where(x => x.Username == username && x.ApplicationName == ApplicationName)
                      .Cacheable()
                      .SingleOrDefault();
                if (usr == null)
                {
                    throw new Exception("User not exist");
                }

            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "UnlockUser");
                _logger.Error(e.Message);
                throw new ProviderException(e.Message);
            }
            return usr;

        }

        private IEnumerable<Users> GetUsers()
        {
            IList<Users> usrs;

            try
            {
                var session = SessionManager.GetCurrentSession();
                usrs = session.QueryOver<Users>().Where(x => x.ApplicationName == ApplicationName)
                    .Cacheable()
                    .List<Users>();

            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUsers");
                _logger.Error(e.Message);
                throw new ProviderException(ExceptionMessage);
            }
            return usrs;

        }

        private IList<Users> GetUsersLikeUsername(string usernameToMatch)
        {
            IList<Users> usrs;

            try
            {
                var session = SessionManager.GetCurrentSession();
                usrs = session.QueryOver<Users>()
                           .Where(x => x.Username == usernameToMatch && x.ApplicationName == ApplicationName)
                           .Cacheable()
                           .List<Users>();

            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUsersMatchByUsername");
                _logger.Error(e.Message);
                throw new ProviderException(ExceptionMessage);
            }
            return usrs;

        }

        private IList<Users> GetUsersLikeEmail(string emailToMatch)
        {
            IList<Users> usrs;
            try
            {
                var session = SessionManager.GetCurrentSession();
                usrs = session.QueryOver<Users>()
                             .Where(x => x.Email == emailToMatch && x.ApplicationName == ApplicationName)
                             .Cacheable()
                             .List<Users>();

            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUsersMatchByEmail");
                _logger.Error(e.Message);
                throw new ProviderException(ExceptionMessage);
            }

            return usrs;

        }
        #endregion

        #region Public methods

        // Initilaize the provider 
        public override void Initialize(string name, NameValueCollection config)
        {
            _logger.Info("UserMembership Provider Initialized.");
            // Initialize values from web.config.
            if (config == null)
                throw new ArgumentNullException("config");

            if (name.Length == 0)
                name = "FNHMemebershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Sample Fluent Nhibernate Membership provider");
            }
            // Initialize the abstract base class.
            base.Initialize(name, config);

            _applicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "-1"));
            _passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            _minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            _minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            _passwordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            _enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            _enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            _requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            _requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            WriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));


            var tempFormat = config["passwordFormat"] ?? "Hashed";

            switch (tempFormat)
            {
                case "Hashed":
                    _passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    _passwordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    _passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }

            //
            // Initialize Connection.
            //connectionString = WebConfigurationManager.ConnectionStrings[GetLoggedInUserName()].ConnectionString; //ConnectionStringSettings.ConnectionString;
            // Get encryption and decryption key information from the configuration.

            //Encryption skipped
            Configuration cfg =
                            WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

            if (_machineKey.ValidationKey.Contains("AutoGenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Encrypted)
                    throw new ProviderException("Hashed or clear passwords are not supported with auto-generated keys.");

            // for gettting session factory

        }

        // Change password for a user
        public override bool ChangePassword(string username, string oldPwd, string newPwd)
        {
            var rowsAffected = 0;
            if (!ValidateUser(username, oldPwd))
                return false;

            var args = new ValidatePasswordEventArgs(username, newPwd, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                {
                    throw args.FailureInformation;
                }
                else
                {
                    _logger.Error("Change password canceled due to new password validation failure.");
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");
                }


            try
            {
                Users usr = GetUserByUsername(username);

                if (usr != null)
                {
                    usr.Password = EncodePassword(newPwd);
                    usr.LastPasswordChangedDate = DateTime.Now;
                    SessionManager.GetCurrentSession().SaveOrUpdate(usr);
                    rowsAffected = 1;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "ChangePassword");

                _logger.Error(e.Message);

                throw new ProviderException(ExceptionMessage);
            }
            return rowsAffected > 0;
        }

        // Change Password Question And Answer for a user
        public override bool ChangePasswordQuestionAndAnswer(string username,
                      string password,
                      string newPwdQuestion,
                      string newPwdAnswer)
        {
            var rowsAffected = 0;
            if (!ValidateUser(username, password))
                return false;


            try
            {
                var usr = GetUserByUsername(username);
                if (usr != null)
                {
                    usr.PasswordQuestion = newPwdQuestion;
                    usr.PasswordAnswer = newPwdAnswer;
                    SessionManager.GetCurrentSession().SaveOrUpdate(usr);
                    rowsAffected = 1;
                }
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");
                _logger.Error(e.Message);
                throw new ProviderException(ExceptionMessage);
            }

            if (rowsAffected > 0)
                return true;
            return false;
        }

        // Create a new Membership user 

        #region

        public void CreateUser(string username, string email, DateTime joiningDate, StkhHierarchy role, out MembershipCreateStatus status)
        {
            SessionManager.GetCurrentSession();
            const string passwordQuestion = "Date of Joining (format: yyyyMMdd e.g. 20131231)?";
            var passwordAnswer = joiningDate.ToString("yyyyMMdd");
             string password = GetEncodePassword("collosys");

            var args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);
            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return;
            }

            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return;
            }

            var u = GetUser(username, false);

            if (u == null)
            {
                DateTime createDate = DateTime.Now;

                //provider user key in our case is auto int
                var session = SessionManager.GetCurrentSession();

                var user = new Users
                    {
                        Username = username,
                        Password = EncodePassword(password),
                        Email = email,
                        PasswordQuestion = passwordQuestion,
                        PasswordAnswer = EncodePassword(passwordAnswer),
                        IsApproved = true,
                        Comment = "",
                        CreatedOn = createDate,
                        LastPasswordChangedDate = createDate,
                        LastActivityDate = createDate,
                        ApplicationName = _applicationName,
                        IsLockedOut = false,
                        LastLockedOutDate = createDate,
                        FailedPasswordAttemptCount = 0,
                        FailedPasswordAttemptWindowStart = createDate,
                        FailedPasswordAnswerAttemptCount = 0,
                        FailedPasswordAnswerAttemptWindowStart = createDate,
                        Role = role
                    };

                try
                {
                    Guid retId;
                        retId = (Guid)session.Save(user);

                    status = retId == Guid.Empty ? MembershipCreateStatus.UserRejected :
                                 MembershipCreateStatus.Success;
                }
                catch (Exception e)
                {
                    status = MembershipCreateStatus.ProviderError;
                    if (WriteExceptionsToEventLog)
                        WriteToEventLog(e, "CreateUser");
                }

                //retrive and return user by user name
                if (username != null) GetUser(username, false);
                return;
            }
            status = MembershipCreateStatus.DuplicateUserName;
        }

        public override MembershipUser CreateUser(string username,
                                                  string password,
                                                  string email,
                                                  string passwordQuestion,
                                                  string passwordAnswer,
                                                  bool isApproved,
                                                  object providerUserKey,
                                                  out MembershipCreateStatus status)
        {
            var args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);
            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            var u = GetUser(username, false);

            if (u == null)
            {
                var createDate = DateTime.Now;

                var user = new Users
                    {
                        Username = username,
                        Password = EncodePassword(password),
                        Email = email,
                        PasswordQuestion = passwordQuestion,
                        PasswordAnswer = EncodePassword(passwordAnswer),
                        IsApproved = isApproved,
                        Comment = "",
                        CreatedOn = createDate,
                        LastPasswordChangedDate = createDate,
                        LastActivityDate = createDate,
                        ApplicationName = _applicationName,
                        IsLockedOut = false,
                        LastLockedOutDate = createDate,
                        FailedPasswordAttemptCount = 0,
                        FailedPasswordAttemptWindowStart = createDate,
                        FailedPasswordAnswerAttemptCount = 0,
                        FailedPasswordAnswerAttemptWindowStart = createDate,
                        Role = new StkhHierarchy()
                    };

                try
                {
                    SessionManager.GetCurrentSession().SaveOrUpdate(user);
                    status = MembershipCreateStatus.Success;
                }
                catch (Exception e)
                {
                    status = MembershipCreateStatus.UserRejected;
                    if (WriteExceptionsToEventLog)
                        WriteToEventLog(e, "CreateUser");
                }

                //retrive and return user by user name
                return GetUser(username, false);
            }
            status = MembershipCreateStatus.DuplicateUserName;
            return null;
        }

        #endregion


        // Delete a user 
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            var rowsAffected = 0;
            var session = SessionManager.GetCurrentSession();

            try
            {
                var usr = GetUserByUsername(username);
                if (usr != null)
                {
                    session.Delete(usr);
                    rowsAffected = 1;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "DeleteUser");
                _logger.Error(e.Message);
                throw new ProviderException(ExceptionMessage);
            }
            return rowsAffected > 0;
        }

        // Get all users in db
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;
            int[] counter = { 0 };
            var startIndex = pageSize * pageIndex;
            var endIndex = startIndex + pageSize - 1;


            var session = SessionManager.GetCurrentSession();
            try
            {
                totalRecords = session.QueryOver<Users>()
                           .Where(x => x.ApplicationName == ApplicationName)
                           .Cacheable()
                           .List<Users>()
                           .Count;

                if (totalRecords <= 0) { return users; }

                var allusers = GetUsers();
                foreach (var u in allusers.TakeWhile(u => counter[0] < endIndex))
                {
                    if (counter[0] >= startIndex)
                    {
                        var mu = GetMembershipUserFromUser(u);
                        users.Add(mu);
                    }
                    counter[0]++;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetAllUsers");
                _logger.Error(e.Message);
                throw new ProviderException(ExceptionMessage);
            }
            return users;
        }

        // Gets a number of online users
        public override int GetNumberOfUsersOnline()
        {
            var onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
            var compareTime = DateTime.Now.Subtract(onlineSpan);
            int numOnline;
            var session = SessionManager.GetCurrentSession();
            try
            {
                numOnline = session.QueryOver<Users>()
                                   .Where(
                                       x =>
                                       x.ApplicationName == ApplicationName &&
                                       x.LastActivityDate == compareTime)
                                   .List<Users>().Count;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetNumberOfUsersOnline");
                _logger.Error(e.Message);
                throw new ProviderException(ExceptionMessage);
            }
            return numOnline;
        }

        // Get a password fo a user
        public override string GetPassword(string username, string answer)
        {
            string password;
            string passwordAnswer;

            if (!EnablePasswordRetrieval)
                throw new ProviderException("Password Retrieval Not Enabled.");

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
                throw new ProviderException("Cannot retrieve Hashed passwords.");

            try
            {
                Users usr = GetUserByUsername(username);

                if (usr == null)
                    throw new MembershipPasswordException("The supplied user name is not found.");
                if (usr.IsLockedOut)
                    throw new MembershipPasswordException("The supplied user is locked out.");

                password = usr.Password;
                passwordAnswer = usr.PasswordAnswer;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetPassword");
                _logger.Error(e.Message);
                throw new ProviderException(ExceptionMessage);
            }

            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer");
                throw new MembershipPasswordException("Incorrect password answer.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
                password = UnEncodePassword(password);

            return password;
        }

        // Get a membership user by username
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return GetMembershipUserByKeyOrUser(false, username, 0, userIsOnline);
        }

        //  // Get a membership user by key ( in our case key is int)
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return GetMembershipUserByKeyOrUser(true, string.Empty, providerUserKey, userIsOnline);
        }

        //Unlock a user given a username 
        public override bool UnlockUser(string username)
        {
            var unlocked = false;
            try
            {
                var usr = GetUserByUsername(username);

                if (usr != null)
                {
                    usr.LastLockedOutDate = DateTime.Now;
                    usr.IsLockedOut = false;
                    SessionManager.GetCurrentSession().SaveOrUpdate(usr);
                    unlocked = true;
                }
            }
            catch (Exception e)
            {
                WriteToEventLog(e, "UnlockUser");
                _logger.Error(e.Message);
                throw new ProviderException(ExceptionMessage);
            }
            return unlocked;
        }

        //Gets a membehsip user by email
        public override string GetUserNameByEmail(string email)
        {
            Users usr;
            var session = SessionManager.GetCurrentSession();
            try
            {
                usr = session.QueryOver<Users>()
                             .Where(x => x.Email == email)
                             .SingleOrDefault<Users>();
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUserNameByEmail");
                _logger.Error(e.Message);
                throw new ProviderException(ExceptionMessage);
            }
            return usr == null ? string.Empty : usr.Username;
        }

        // Reset password for a user
        public override string ResetPassword(string username, string answer)
        {
            int rowsAffected;

            if (!EnablePasswordReset)
                throw new NotSupportedException("Password reset is not enabled.");

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer");
                throw new ProviderException("Password answer required for password reset.");
            }

            string newPassword =
                            System.Web.Security.Membership.GeneratePassword(NewPasswordLength, MinRequiredNonAlphanumericCharacters);


            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");

            try
            {
                var usr = GetUserByUsername(username);

                if (usr == null)
                    throw new MembershipPasswordException("The supplied user name is not found.");

                if (usr.IsLockedOut)
                    throw new MembershipPasswordException("The supplied user is locked out.");

                var session = SessionManager.GetCurrentSession();
                var users = session.QueryOver<Users>()
                           .Where(x => x.Username == username)
                           .SingleOrDefault();

                if (users == null)
                {
                    throw new MemberAccessException("User is not authorized to access the system.");
                }

                if (RequiresQuestionAndAnswer && !CheckAnswer(answer, users.PasswordAnswer))
                {
                    UpdateFailureCount(username, "passwordAnswer");
                    throw new MembershipPasswordException("Incorrect password answer.");
                }

                usr.Password = EncodePassword(newPassword);
                usr.LastPasswordChangedDate = DateTime.Now;
                usr.Username = username;
                usr.ApplicationName = ApplicationName;
                SessionManager.GetCurrentSession().SaveOrUpdate(usr);
                rowsAffected = 1;
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "ResetPassword");
                _logger.Error(e.Message);
                throw new ProviderException(ExceptionMessage);
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "UserNotExist");
                _logger.Error(e.Message);
                throw new Exception("User not found or blocked.Please try with valid user.");
            }

            if (rowsAffected > 0)
                return newPassword;
            throw new MembershipPasswordException("User not found or blocked.Please try with valid user.");
        }


        //check answer of user question
        private static bool CheckAnswer(string answer, string passwordanswer)
        {
            if (answer == passwordanswer)
            {
                return true;
            }

            throw new Exception("Information does not match");
        }

        //update user role
        /*
                public void UpdateUserRole(MembershipUser user, StakeHierarchy role)
                {
                    try
                    {
                        var usr = GetUserByUsername(user.UserName);
                        if (usr == null) return;
                        usr.Role = role;
                        SessionManager.GetAutomicDao<Users>().SaveOrUpdate(usr);
                    }
                    catch (Exception e)
                    {
                        if (!WriteExceptionsToEventLog) return;
                        WriteToEventLog(e, "UpdateUser");
                        throw new ProviderException(ExceptionMessage);
                    }
                }
        */

        // Update a user information 
        public override void UpdateUser(MembershipUser user)
        {
            try
            {
                Users usr = GetUserByUsername(user.UserName);
                if (usr == null) return;
                usr.Email = user.Email;
                usr.Comment = user.Comment;
                usr.IsApproved = user.IsApproved;
                SessionManager.GetCurrentSession().SaveOrUpdate(usr);
            }
            catch (Exception e)
            {
                if (!WriteExceptionsToEventLog) return;
                WriteToEventLog(e, "UpdateUser");
                _logger.Error(e.Message);
                throw new ProviderException(ExceptionMessage);
            }
        }

        // Validates as user
        public override bool ValidateUser(string username, string password)
        {
            var isValid = false;
            try
            {
                Users usr = GetUserByUsername(username);
                if (usr == null)
                    return false;
                if (usr.IsLockedOut)
                    return false;

                if (CheckPassword(password, usr.Password))
                {
                    if (usr.IsApproved)
                    {
                        isValid = true;
                        usr.LastLoginDate = DateTime.Now;
                        SessionManager.GetCurrentSession().SaveOrUpdate(usr);
                    }
                }
                else
                    UpdateFailureCount(username, "password");
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ValidateUser");
                    _logger.Error(e.Message);
                    throw new ProviderException(ExceptionMessage);
                }
                throw;
            }
            return isValid;
        }

        // Find users by a name, note : does not do a like search
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            var counter = 0;
            var startIndex = pageSize * pageIndex;
            var endIndex = startIndex + pageSize - 1;
            totalRecords = 0;

            try
            {
                var allusers = GetUsersLikeUsername(usernameToMatch);
                if (allusers == null)
                    return users;
                if (allusers.Count > 0)
                    totalRecords = allusers.Count;
                else
                    return users;

                foreach (Users u in allusers)
                {
                    if (counter >= endIndex)
                        break;
                    if (counter >= startIndex)
                    {
                        MembershipUser mu = GetMembershipUserFromUser(u);
                        users.Add(mu);
                    }
                    counter++;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByName");
                    _logger.Error(e.Message);
                    throw new ProviderException(ExceptionMessage);
                }
                throw;
            }
            return users;
        }

        // Search users by email , NOT a Like match
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            var counter = 0;
            var startIndex = pageSize * pageIndex;
            var endIndex = startIndex + pageSize - 1;
            totalRecords = 0;

            try
            {
                var allusers = GetUsersLikeEmail(emailToMatch);
                if (allusers == null)
                    return users;
                if (allusers.Count > 0)
                    totalRecords = allusers.Count;
                else
                    return users;

                foreach (Users u in allusers)
                {
                    if (counter >= endIndex)
                        break;
                    if (counter >= startIndex)
                    {
                        MembershipUser mu = GetMembershipUserFromUser(u);
                        users.Add(mu);
                    }
                    counter++;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByEmail");
                    _logger.Error(e.Message);
                    throw new ProviderException(ExceptionMessage);
                }
                throw;
            }
            return users;
        }

        #endregion
    }
}
