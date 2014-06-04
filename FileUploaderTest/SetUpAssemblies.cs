using System;
using System.Configuration;
using System.IO;
using ColloSys.DataLayer.NhSetup;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.UserInterface.Areas.Developer.Models.Excel2Db;
using ColloSys.Shared.ConfigSectionReader;
using NHibernate;
using NHibernate.Context;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NUnit.Framework;
//using ReflectionExtension.Tests.FileReaderTest;

namespace ReflectionExtension.Tests
{
    [SetUpFixture]
    public class SetUpAssemblies
    {
        protected readonly FileStream FileStream;
        protected readonly FileInfo FileInfo;

        public SetUpAssemblies()
        {
            FileStream = ResourceReader.GetEmbeddedResourceAsFileStream("DrillDown_Txn_1.xls");
            FileInfo = new FileInfo(FileStream.Name);
            InitNhibernate();
            NLogConfig.InitConFig(ColloSysParam.WebParams.LogPath, ColloSysParam.WebParams.LogLevel);
        }

        private void InitNhibernate()
        {
            var connectionString = new ConnectionStringSettings("sqlte", 
                string.Format("Data Source='{0}';Version=3;", ":memory:"));
            var obj = new NhInitParams { ConnectionString = connectionString, 
                DbType = ConfiguredDbTypes.SqLite, 
                IsWeb = false };

            SessionManager.InitNhibernate(obj);
        }
        public static class NLogConfig
        {
            private static bool _hasInitialized;
            public static void InitConFig(string logPath, LogLevel level)
            {
                if (_hasInitialized) return;

                var config = new LoggingConfiguration();

                var fileTarget = FileTarget(logPath);
                config.AddTarget("FileUploadService", fileTarget);
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
                    FileName = logPath + "FileUploadService_${date:format = yyyyMMddHH}_" + time + ".csv",
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
}
