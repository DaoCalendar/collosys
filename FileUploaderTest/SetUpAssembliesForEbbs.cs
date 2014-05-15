using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.NhSetup;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.Shared.ConfigSectionReader;
using ColloSys.UserInterface.Areas.Developer.Models.Excel2Db;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Exceptions;
using NHibernate.Mapping;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace ReflectionExtension.Tests
{
    [SetUpFixture]
    public class SetUpAssembliesForTest
    {

        protected readonly FileStream FileStream;
        protected readonly FileInfo FileInfo;
        private static ConnectionStringSettings _connectionString;
        private static int _initCount;
        private static ConfiguredDbTypes _dbType;
        private NHibernate.Cfg.Configuration _cfg;
        public ISessionFactory _sessionFactory;
        public ISession _session;
        public SetUpAssembliesForTest()
        {
            FileStream = ResourceReader.GetEmbeddedResourceAsFileStream("AEB Auto Charge Off Base - 28.01.2014.xls");
            FileInfo = new FileInfo(FileStream.Name);
         
           InitSqlite();
        }

        public void InitNhibernate()
        {
            if (_initCount++ != 0) return;
            _dbType = ConfiguredDbTypes.SqLite;
            _connectionString = ColloSysParam.WebParams.ConnectionString;
            var obj = new NhInitParams { ConnectionString = _connectionString, DbType = _dbType, IsWeb = false };

            SessionManager.InitNhibernate(obj);
            SessionManager.BindNewSession();
            new SchemaExport(SessionManager.GetNhConfiguration()).Execute(true,true,false);
        }

        public void InitSqlite()
        {

            if (_cfg == null)
            {
                _cfg = new NHibernate.Cfg.Configuration().DataBaseIntegration(db =>
                {
                    db.Dialect<ExtendedSqliteDialect>();
                    db.Driver<ExtendedSqliteDriver>();
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
              
                
                _sessionFactory = _cfg.BuildSessionFactory();

                MappingSetup();
               // _cfg.AddAssembly(typeof(Entitys).Assembly);

            }
           
           _session = _sessionFactory.OpenSession();
            
            new SchemaExport(_cfg).Execute(true, true, false,_session.Connection,null);
            
            _session
            using (var sess = _sessionFactory.OpenSession())
            {


                using (var tx = sess.BeginTransaction())
                {
                    var obj = new Entitys {Name = "algo"};
                    _session.Save(obj);
                    tx.Commit();
                }
            }

            //var  file = _session
          //          .QueryOver<FileScheduler>()
          //          .Where(c => c.FileDetail.Id == new Guid("A42EF611-808D-4CC2-9F6F-D15069664D4C"))
          //          .List<FileScheduler>().FirstOrDefault();

            //_serviceFactory = new ServiceFactory(() => _session);

            //var connection = _session.Connection;
            //using (var command = connection.CreateCommand())
            //{
            //    // Activated foreign keys if supported by SQLite.  Unknown pragmas are ignored.
            //    command.CommandText = "PRAGMA foreign_keys = ON";
            //    command.ExecuteNonQuery();
            //}

            

        }

        private void MappingSetup()
        {
            var mapper = new ModelMapper();

            // add all mappings in executing assembly
            mapper.AddMappings(Assembly.GetAssembly(typeof (Entitys)).GetTypes());
           // mapper.AddMapping<EntityMap>();

            //HbmMapping mapping = mapper.CompileMappingFor(new[] {typeof (Entitys)});
            // convert them to hbm
            var domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            // add mappings to current configuration
            _cfg.AddMapping(domainMapping);
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

    public  class Entitys
    {
        public Entitys()
        {
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class EntityMap : ClassMapping<Entitys>
    {
        public EntityMap()
        {
            Id<Guid>(x => x.Id, map =>
            {
                map.Generator(Generators.GuidComb);
                map.UnsavedValue(Guid.Empty);
            });
            Property<string>(x=>x.Name);
        }

    }
}


