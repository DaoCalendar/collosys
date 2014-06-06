#region references

using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.SqlServer;

#endregion


namespace ColloSys.DataLayer.Migrate
{
    public static class MigrateRunner
    {
        public static void MigrateToLatest(string connectionString)
        {
            var announcer = new TextWriterAnnouncer(s => Trace.WriteLine(s))
                {
                    ShowElapsedTime = true,
                    ShowSql = true
                };
            var assembly = Assembly.GetExecutingAssembly();

            var migrationContext = new RunnerContext(announcer) { Target = assembly.FullName };
            var options = new MigrationOptions { PreviewOnly = false, Timeout = 60 };
            var factory = new SqlServer2008ProcessorFactory();
            var processor = factory.Create(connectionString, announcer, options);

            var runner = new MigrationRunner(assembly, migrationContext, processor);
            runner.MigrateUp(true);
        }

        private static long MigrateToLatestQuery(string connectionString)
        {
            var announcer = new TextWriterAnnouncer(s => Trace.WriteLine(s));
            var assembly = Assembly.GetExecutingAssembly();
            var migrationContext = new RunnerContext(announcer) { Target = assembly.FullName };
            var options = new MigrationOptions { PreviewOnly = true, Timeout = 60 };
            var factory = new SqlServer2008ProcessorFactory();
            var processor = factory.Create(connectionString, announcer, options);
            var runner = new MigrationRunner(assembly, migrationContext, processor);
            var migs = runner.MigrationLoader.LoadMigrations();
            var maxno = migs.Keys.Max();
            return maxno;
        }

        public static string VersionInfoTableCreate()
        {
            return @"create table VersionInfo ( [Version] bigint primary key  , AppliedOn datetime2 )";
        }

        public static string VersionInfoTableInsert(string connectionstring)
        {
            var version = MigrateToLatestQuery(connectionstring);
            if (version == 0) return string.Empty;
            var insertInto = string.Format("insert into VersionInfo(\"Version\",\"AppliedOn\") values({0}, GETDATE())",
                                           version);
            return insertInto;
        }

        private class MigrationOptions : IMigrationProcessorOptions
        {
            public MigrationOptions(string providerSwitches = "")
            {
                ProviderSwitches = providerSwitches;
            }

            public bool PreviewOnly { get; set; }
            public int Timeout { get; set; }
            public string ProviderSwitches { get; private set; }
        }

    }
}