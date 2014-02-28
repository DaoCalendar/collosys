#region References
using System.Diagnostics;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
#endregion

namespace ColloSys.DataLayer.Infra.NhSetup
{
    internal static class LoggingSetup
    {
        public static void SetupLogging()
        {
            ConfigureLog4Net();

            var log = LogManager.GetLogger("NHibernate.SQL");
            log.Fatal("Starting Nhibernate SQL Logging!!!");

            var log2 = LogManager.GetLogger("NHibernate");
            log2.Fatal("Starting Nhibernate Logging!!!");

            var log3 = LogManager.GetLogger("NHibernate.Envers");
            log3.Fatal("Starting Nhibernate Envers Logging!!!");

            var log4 = LogManager.GetLogger("NHibernate.Validator");
            log4.Fatal("Starting Nhibernate Validator Logging!!!");
        }

        private static Hierarchy GetHierarchy()
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders();
            var rootLogger = hierarchy.Root;
            rootLogger.Level = Level.Debug;

            return hierarchy;
        }

        private static RollingFileAppender GetRollingAppender()
        {
            var appenderNh = new RollingFileAppender
                {
                    Name = "RollingLogFileAppenderNHibernate",
                    AppendToFile = true,
                    MaximumFileSize = "5MB",
                    MaxSizeRollBackups = 10,
                    RollingStyle = RollingFileAppender.RollingMode.Size,
                    StaticLogFileName = true,
                    LockingModel = new FileAppender.MinimalLock(),
                    File = @"C:\Users\Public\Documents\log-nhibernate.log",
                    Layout = new PatternLayout(
                        "%date [%thread] %logger %-5level [%ndc] - %message%newline")
                };
            appenderNh.ActivateOptions();

            return appenderNh;
        }

        private static void ConfigureLog4Net()
        {
            var hierarchy = GetHierarchy();
            var appenderNh = GetRollingAppender();


            var loggerNh = hierarchy.GetLogger("NHibernate.SQL") as Logger;
            Debug.Assert(loggerNh != null, "loggerNh != null");
            loggerNh.Level = Level.Debug;
            loggerNh.AddAppender(appenderNh);

            var logger = hierarchy.GetLogger("NHibernate") as Logger;
            Debug.Assert(logger != null, "logger != null");
            logger.Level = Level.Warn;
            logger.AddAppender(appenderNh);

            var loggerEnv = hierarchy.GetLogger("NHibernate.Envers") as Logger;
            Debug.Assert(loggerEnv != null, "loggerEnv != null");
            loggerEnv.Level = Level.Error;
            loggerEnv.AddAppender(appenderNh);

            var loggerValidator = hierarchy.GetLogger("NHibernate.Validator") as Logger;
            Debug.Assert(loggerValidator != null, "loggerEnv != null");
            loggerValidator.Level = Level.Error;
            loggerValidator.AddAppender(appenderNh);

            hierarchy.Configured = true;
        }

        
    }
}
