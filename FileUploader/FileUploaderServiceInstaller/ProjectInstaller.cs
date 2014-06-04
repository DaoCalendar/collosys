using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ColloSys.FileUploaderService.v2.FileUploaderServiceInstaller
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public const string ColloSysServiceName = "FileUploaderService";

        public ProjectInstaller()
        {
            AfterInstall += ServiceInstaller_AfterInstall;
            BeforeUninstall += ServiceInstaller_BeforeUninstall;

            var processInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };

            var serviceInstaller = new ServiceInstaller
            {
                DisplayName = ColloSysServiceName,
                ServiceName = ColloSysServiceName,
                StartType = ServiceStartMode.Automatic
            };

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }

        void ServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            var sc = new ServiceController(ColloSysServiceName);
            if (sc.Status != ServiceControllerStatus.Stopped)
            {
                sc.Stop();
            }
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            sc.Start();
            sc.Refresh();
        }

        void ServiceInstaller_BeforeUninstall(object sender, InstallEventArgs e)
        {
            var sc = new ServiceController(ColloSysServiceName);
            if (sc.Status != ServiceControllerStatus.Stopped)
            {
                sc.Stop();
            }

            sc.Refresh();
        }
    }
    
}

