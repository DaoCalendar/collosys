using System;
using System.Configuration;
using System.Configuration.Install;
using System.Globalization;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Timers;
using ColloSys.FileUploaderService.Logging;
using ColloSys.Shared.ConfigSectionReader;
using NLog;

namespace ColloSys.FileUploaderServiceInstaller
{
    public partial class FileUploaderWinService : ServiceBase
    {
        readonly Timer _timer = new Timer();
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private ServiceHost _serviceHost;

        private FileUploaderWinService()
        {
            ConnectionStringSettings connString = ColloSysParam.WebParams.ConnectionString;
            ServiceName = ProjectInstaller.ColloSysServiceName;
            _log.Info(string.Format("Allocation Service: Connection String : {0}", connString.ConnectionString));
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
           
            NLogConfig.InitConFig(ColloSysParam.WebParams.LogPath, ColloSysParam.WebParams.LogLevel);
        }

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
                Run(new FileUploaderWinService());
            }
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //var logfile = ColloSysParam.WebParams.LogPath + @"\error.txt";
            //File.AppendAllText(logfile, ((Exception)e.ExceptionObject).Message + ((Exception)e.ExceptionObject).InnerException.Message);
            var log = LogManager.GetCurrentClassLogger();
            log.Error("Service: Unhandled Exception : " + e.ExceptionObject);
            
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            _timer.Elapsed += OnEventTime;

            _timer.Interval = 60000;

            _timer.Enabled = true;

            _log.Info("Service: File uploader service started.");
        }

        protected override void OnStop()
        {
            _timer.Enabled = false;

            _log.Info("Service: File uploader service stoped.");
            if (_serviceHost == null) return;
            _serviceHost.Close();
            _serviceHost = null;
        }

        private static bool HasRunBefore { get; set; }

        private void OnEventTime(object source, ElapsedEventArgs e)
        {
            _log.Debug("Service: in timer event");

            IFileUploadService fileUpload=new FileUploadService();
            
            if (!HasRunBefore)
            {
                fileUpload.ResetFiles();
                HasRunBefore = true;
            }
            _log.Info("going to uploaded file");
            fileUpload.UploadFiles();
        }
    }
}
