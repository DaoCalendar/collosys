﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ColloSys.FileUploaderService.Logging;
using ColloSys.FileUploadServiceInstaller;
using ColloSys.Shared.ConfigSectionReader;
using NLog;

namespace FileUploadServiceInstallerv2
{
    public partial class FileUploaderServiceBase : ServiceBase
    {
           private readonly Logger _log = LogManager.GetCurrentClassLogger();
        public ServiceHost ServiceHost;

        public FileUploaderServiceBase()
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
                Run(new FileUploaderServiceBase());
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

        #region start

        private static Timer _aTimer;
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            _aTimer = new Timer(10000);
            _aTimer.Elapsed += OnTimedEvent;
            _aTimer.Interval = 60 * 1000;
            _aTimer.Enabled = true;

            _log.Info("Service: File upload service started.");
        }

        private static bool HasRunBefore { get; set; }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _log.Debug("Service: in timer event");

            IFileUploadService fileUpload = new FileUploadService();
            if (!HasRunBefore)
            {
                fileUpload.ResetFiles();
                HasRunBefore = true;
            }
            fileUpload.UploadFiles();
        }

        #endregion

        #region stop

        protected override void OnStop()
        {
            _log.Info("Service: File Upload service is stopping.");
            if (ServiceHost == null) return;
            ServiceHost.Close();
            ServiceHost = null;
        }

        protected override void OnShutdown()
        {
            _log.Info("Service: File Upload service is shutting down.");
            base.OnShutdown();
        }

        #endregion

        //public Service1()
        //{
        //    InitializeComponent();
        //}

        //protected override void OnStart(string[] args)
        //{
        //}

        //protected override void OnStop()
        //{
        //}
    }
}
