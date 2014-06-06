using System;
using System.Configuration.Install;
using System.Globalization;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Timers;
using ColloSys.EMailServices;
using ColloSys.EMailServices.Logging;
using ColloSys.Shared.ConfigSectionReader;
using NLog;

namespace ColloSys.EmailWindowService
{
    public partial class EmailReportService : ServiceBase
    {
        public ServiceHost ServiceHost;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public EmailReportService()
        {
            ServiceName = ProjectInstaller.ColloSysServiceName;
            NLogConfig.InitConFig(ColloSysParam.WebParams.LogPath, ColloSysParam.WebParams.LogLevel);
        }

        #region main

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            if (Environment.UserInteractive)
            {
                var parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("The parameter:'{0}' is not supported", parameter));
                }
            }
            else
            {
                Run(new EmailReportService());
            }
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //var logfile = ColloSysParam.WebParams.LogPath + @"\error.txt";
            //File.AppendAllText(logfile, ((Exception)e.ExceptionObject).Message + ((Exception)e.ExceptionObject).InnerException.Message);
            var log = LogManager.GetCurrentClassLogger();
            log.Error("Service: Unhandled Exception : " + e.ExceptionObject);
        }

        #endregion


        private static Timer _aTimer;
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            _aTimer = new Timer(10000);
            _aTimer.Elapsed += OnTimedEvent;
            _aTimer.Interval = 60 * 1000;
            _aTimer.Enabled = true;

            _log.Info("Service: Email service started.");
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _log.Debug("Service: in timer event");

            var emailService = new EmailService();
            emailService.EmailReports();
        }

        protected override void OnStop()
        {
            _log.Info("Service: Email service is stopping.");
            if (ServiceHost == null) return;

            ServiceHost.Close();
            ServiceHost = null;
        }

        protected override void OnShutdown()
        {
            _log.Info("Service: Email service is shutting down.");
            base.OnShutdown();
        }
    }
}
