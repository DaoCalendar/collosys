#region references

using System.IO;
using ColloSys.Shared.ConfigSectionReader;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

#endregion

namespace ColloSys.UserInterface.App_Start
{
    public static class NLogConfig
    {
        private static bool _hasInitialized;
        public static void InitConFig()
        {
            if (_hasInitialized) return;

            var logPath = ColloSysParam.WebParams.LogPath;
            var level = ColloSysParam.WebParams.LogLevel;
            var config = new LoggingConfiguration();

            var fileTarget = FileTarget(logPath);
            config.AddTarget("UserInterface", fileTarget);
            SimpleConfigurator.ConfigureForTargetLogging(fileTarget, level);

            var fileTargetRule = new LoggingRule("*", level, fileTarget);
            config.LoggingRules.Add(fileTargetRule);

            LogManager.Configuration = config;
            _hasInitialized = true;
            LogManager.GetCurrentClassLogger().Info("******** LOGGER INITIALIZED **********");
        }

        private static FileTarget FileTarget(string logPath)
        {
            var layout = new CsvLayout();
            layout.Columns.Add(new CsvColumn { Name = "Date", Layout = "${date:format = yyyyMMdd}" });
            layout.Columns.Add(new CsvColumn { Name = "Time", Layout = "${date:format = HHmmss}" });
            layout.Columns.Add(new CsvColumn { Name = "User", Layout = "${aspnet-user-identity}" });
            layout.Columns.Add(new CsvColumn { Name = "Level", Layout = "${level:uppercase=true}" });
            layout.Columns.Add(new CsvColumn { Name = "Logger", Layout = "${logger}" });
            layout.Columns.Add(new CsvColumn { Name = "Message", Layout = "${message}" });

            //https://github.com/nlog/NLog/wiki/Layout-Renderers
            var fileTarget = new FileTarget
            {
                FileName = logPath + "UserInterface_${date:format = yyyyMMdd}.csv",
                CreateDirs = true,
                Layout = layout,
                MaxArchiveFiles = 180,
                ArchiveEvery = FileArchivePeriod.Month,
                FileAttributes = (Win32FileAttributes)FileAttributes.ReadOnly,
                AutoFlush = true,
                ConcurrentWrites = true,
                KeepFileOpen = true
            };

            return fileTarget;
        }
    }
}