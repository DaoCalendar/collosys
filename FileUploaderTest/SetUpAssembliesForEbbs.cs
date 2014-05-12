using System;
using System.Configuration;
using System.Data;
using System.IO;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.NhSetup;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.Shared.ConfigSectionReader;
using ColloSys.UserInterface.Areas.Developer.Models.Excel2Db;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace ReflectionExtension.Tests
{
     [SetUpFixture]
    class SetUpAssembliesForTest
    {
        
        protected readonly FileStream FileStream;
        protected readonly FileInfo FileInfo;
        private static ConnectionStringSettings _connectionString;
        private static int _initCount;
        private static ConfiguredDbTypes _dbType;
       private NHibernate.Cfg.Configuration _cfg;
         public  ISessionFactory _sessionFactory;
         public  ISession _session;
        public SetUpAssembliesForTest()
        {
            FileStream = ResourceReader.GetEmbeddedResourceAsFileStream("AEB Auto Charge Off Base - 28.01.2014.xls");
            FileInfo=new FileInfo(FileStream.Name);
            InitSqlite();
        }

        private void InitNhibernate()
        {
            if (_initCount++ != 0) return;
            _dbType = ConfiguredDbTypes.SqLite;
            _connectionString = ColloSysParam.WebParams.ConnectionString;
            var obj = new NhInitParams { ConnectionString = _connectionString, DbType = _dbType, IsWeb = false };

            SessionManager.InitNhibernate(obj);
            SessionManager.BindNewSession();
        }
       
        public void InitSqlite()
        {
           
            if (_cfg == null)
            {
                _cfg = new NHibernate.Cfg.Configuration().DataBaseIntegration(db=>
                {  
                 db.Dialect<SQLiteDialect>();
                 db.Driver<SQLite20Driver>();
                 db.ConnectionString = string.Format("Data Source='{0}';Version=3;", ":memory:");
                   
                 db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                 db.IsolationLevel = IsolationLevel.ReadCommitted;
                 db.Timeout = 10;

                 db.LogFormattedSql = true;
                 db.LogSqlInConsole = true;
                 db.AutoCommentSql = true;

                 db.BatchSize = 1000;
                 db.SchemaAction = SchemaAutoAction.Validate;
             });
                _cfg.SessionFactory().GenerateStatistics();
               //SetProperty(NHibernate.Cfg.Environment.ReleaseConnections, "on_close")
               //     .SetProperty(NHibernate.Cfg.Environment.Dialect, typeof(SQLiteDialect).AssemblyQualifiedName)
               //     .SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, typeof(SQLite20Driver).AssemblyQualifiedName)
               //     .SetProperty(NHibernate.Cfg.Environment.ConnectionString, "data source=:memory:")
               //     .SetProperty(NHibernate.Cfg.Environment.ShowSql, "false")
               //     .SetProperty(NHibernate.Cfg.Environment.GenerateStatistics, "false")
               //     .SetProperty(NHibernate.Cfg.Environment.BatchSize, "10")
               //     .SetProperty(NHibernate.Cfg.Environment.CommandTimeout, "60")
               //     .SetProperty(NHibernate.Cfg.Environment.Hbm2ddlAuto, "create")
               //     .SetProperty(NHibernate.Cfg.Environment.QuerySubstitutions, "true 1, false 0, yes 'Y', no 'N'")
               //     .AddAssembly("Common.Data");

                _sessionFactory = _cfg.BuildSessionFactory();
            }

            _session = _sessionFactory.OpenSession();
            //_serviceFactory = new ServiceFactory(() => _session);

            var connection = _session.Connection;
            using (var command = connection.CreateCommand())
            {
                // Activated foreign keys if supported by SQLite.  Unknown pragmas are ignored.
                command.CommandText = "PRAGMA foreign_keys = ON";
                command.ExecuteNonQuery();
            }

            new SchemaExport(_cfg).Execute(true, true, false, _session.Connection, Console.Out);

        }

        [TearDown]
        public void TearDown()
        {
            if (--_initCount == 0)
            {
                SessionManager.UnbindSession();
            }
        }
    }
}
    

