#region references

using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

#endregion

namespace BillingService2.Logging
{
    public static class NLogConfig
    {
        private static bool _hasInitialized;
        public static void InitConFig(string logPath, LogLevel level)
        {
            if (_hasInitialized) return;

            var config = new LoggingConfiguration();

            var fileTarget = FileTarget(logPath);
            config.AddTarget("BillingService", fileTarget);
            SimpleConfigurator.ConfigureForTargetLogging(fileTarget, level);

            var fileTargetRule = new LoggingRule("*", level, fileTarget);
            config.LoggingRules.Add(fileTargetRule);

            LogManager.Configuration = config;
            _hasInitialized = true;
        }

        private static FileTarget FileTarget(string logPath)
        {
            var time = DateTime.Now.ToString("HHmmssfff");
            var layout = new CsvLayout();
            layout.Columns.Add(new CsvColumn { Name = "Date", Layout = "${date:format = yyyyMMdd}" });
            layout.Columns.Add(new CsvColumn { Name = "Time", Layout = "${date:format = HHmmss}" });
            layout.Columns.Add(new CsvColumn { Name = "Level", Layout = "${level:uppercase=true}" });
            layout.Columns.Add(new CsvColumn { Name = "Logger", Layout = "${logger}" });
            layout.Columns.Add(new CsvColumn { Name = "Message", Layout = "${message}" });

            //https://github.com/nlog/NLog/wiki/Layout-Renderers
            var fileTarget = new FileTarget
            {
                FileName = logPath + "BillingService_${date:format = yyyyMMddHH}_" + time +".csv",
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