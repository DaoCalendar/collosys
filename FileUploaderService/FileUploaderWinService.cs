using System;
using System.Configuration.Install;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Timers;
using System.ServiceProcess;
using ColloSys.FileUploadService.Logging;
using ColloSys.Shared.ConfigSectionReader;
using NLog;

namespace FileUploaderService
{
    public partial class FileUploaderWinService : ServiceBase
    {
        readonly Timer _timer = new Timer();
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
       
        private ServiceHost _serviceHost;
        public FileUploaderWinService()
        {
            ServiceName = ProjectInstaller.ColloSysServiceName;
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
            TraceService("Service Start ");

            _timer.Elapsed += OnEventTime;

            _timer.Interval = 30000;

            _timer.Enabled = true;

            _log.Info("Service: File uploader service started.");
        }

        protected override void OnStop()
        {
            _timer.Enabled = false;

            TraceService("Service stoped");

            _log.Info("Service: File uploader service stoped.");
            if (_serviceHost == null) return;
            _serviceHost.Close();
            _serviceHost = null;
        }

        private static bool HasRunBefore { get; set; }
        public void OnEventTime(object source, ElapsedEventArgs e)
        {
            TraceService("Another entry at " + DateTime.Now);
            _log.Debug("Service: in timer event");

            IFileUploadService fileUpload = new FileUploadService();
            if (!HasRunBefore)
            {
                fileUpload.ResetFiles();
                HasRunBefore = true;
            }
            _log.Info("going to uploaded file");
            fileUpload.UploadFiles();
        }

        private void TraceService(string content)
        {
            var fs = new FileStream(@"c:\FileUploaderService.txt", FileMode.OpenOrCreate, FileAccess.Write);

            var sw = new StreamWriter(fs);

            //find the end of the underlying filestream
            sw.BaseStream.Seek(0, SeekOrigin.End);

            //add the text
            sw.WriteLine(content);
            //add the text to the underlying filestream

            sw.Flush();
            //close the writer
            sw.Close();
        }
    }
}
