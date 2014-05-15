#region references

using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Dialect;
using NHibernate.Driver;

#endregion

namespace ColloSys.DataLayer.NhSetup
{

    public enum ConfiguredDbTypes
    {
        Oracle,
        MsSql,
        SqLite
    }

    internal static class ConfigurationSetup
    {
        public static Configuration ConfigureNHibernate(NhInitParams obj)
        {

            if (obj == null)
            {
                throw new InvalidProgramException("Please pass valid NhInitParams.");
            }

            if (obj.ConnectionString == null)
            {
                throw new InvalidProgramException("Please pass valid NhInitParams/ConnectionString.");
            }

            Configuration cfg;
            switch (obj.DbType)
            {
                case ConfiguredDbTypes.MsSql:
                    cfg = ConfigureMsSql(obj.ConnectionString.ConnectionString);
                    break;
                case ConfiguredDbTypes.Oracle:
                    cfg = ConfigureOracle(obj.ConnectionString.ConnectionString);
                    break;
                case ConfiguredDbTypes.SqLite:
                    cfg = ConfigureSqLite(obj.ConnectionString.ConnectionString);
                    break;
                default:
                    throw new NotImplementedException("Database type not configured.");
            }

            AddListners(cfg);
            return (obj.IsWeb ? cfg.CurrentSessionContext<WebSessionContext>()
                            : cfg.CurrentSessionContext<ThreadStaticSessionContext>());
        }

        private static void AddListners(Configuration cfg)
        {
            var auditor = new AuditEventListener();
            var listInsert = cfg.EventListeners.PreInsertEventListeners.ToList();
            listInsert.Add(auditor);
            cfg.EventListeners.PreInsertEventListeners = listInsert.ToArray();

            var listUpdate = cfg.EventListeners.PreUpdateEventListeners.ToList();
            listUpdate.Add(auditor);
            cfg.EventListeners.PreUpdateEventListeners = listUpdate.ToArray();

            var listDelete = cfg.EventListeners.PreDeleteEventListeners.ToList();
            listDelete.Add(auditor);
            cfg.EventListeners.PreDeleteEventListeners = listDelete.ToArray();
        }

        [Conditional("DEBUG")]
        // ReSharper disable RedundantAssignment
        private static void GetValidationActionDebug(ref SchemaAutoAction action)
        // ReSharper restore RedundantAssignment
        {
            action = SchemaAutoAction.Validate;
        }

        [Conditional("RELEASE")]
        // ReSharper disable RedundantAssignment
        private static void GetValidationActionRelease(ref SchemaAutoAction action)
        // ReSharper restore RedundantAssignment
        {
            action = SchemaAutoAction.Validate;
        }

        private static Configuration ConfigureMsSql(string connection)
        {
            // ReSharper disable RedundantAssignment
            //var dovalidate = SchemaAutoAction.Create;
            //GetValidationActionDebug(ref dovalidate);
            //GetValidationActionRelease(ref dovalidate);
            // ReSharper restore RedundantAssignment

            var config = new Configuration().DataBaseIntegration(db =>
            {
                db.Dialect<ExtendedMsSql2008Dialect>();
                db.Driver<ExtendedSql2008ClientDriver>();
                db.ConnectionString = connection;

                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                db.IsolationLevel = IsolationLevel.ReadCommitted;
                db.Timeout = 10;

                db.LogFormattedSql = true;
                db.LogSqlInConsole = true;
                db.AutoCommentSql = false;

                db.BatchSize = 1000;
                //db.SchemaAction = dovalidate;
            });

            config.SessionFactory().GenerateStatistics();

            return config;
        }

        private static Configuration ConfigureOracle(string connection)
        {
            return new Configuration().DataBaseIntegration(db =>
            {
                db.Dialect<Oracle10gDialect>();
                db.Driver<OracleDataClientDriver>();
                db.ConnectionString = connection;

                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                db.IsolationLevel = IsolationLevel.ReadCommitted;
                db.Timeout = 10;

                db.LogFormattedSql = true;
                db.LogSqlInConsole = true;
                db.AutoCommentSql = false;

                db.BatchSize = 1000;
                db.SchemaAction = SchemaAutoAction.Validate;
            });
        }

        private static Configuration ConfigureSqLite(string connection)
        {
            var config = new Configuration().DataBaseIntegration(db =>
             {
                 db.Dialect<ExtendedSqliteDialect>();

                 db.Driver<ExtendedSqliteDriver>();
                 db.ConnectionString = connection;
                 db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                 db.IsolationLevel = IsolationLevel.ReadCommitted;
                 db.Timeout = 10;

                 db.LogFormattedSql = true;
                 db.LogSqlInConsole = true;
                 db.AutoCommentSql = true;

                 db.BatchSize = 1000;
                 //db.SchemaAction = SchemaAutoAction.Validate;
             });

            //config.SessionFactory().GenerateStatistics();
            return config;
        }
    }
}
