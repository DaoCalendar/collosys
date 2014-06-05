#region references

using System;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;

#endregion

namespace ColloSys.DataLayer.NhSetup
{
    public static class SessionFactoryManager
    {
        private static Configuration _configuration;

        public static NhInitParams NhInitParams { get; private set; }

        public static Configuration NhConfiguration
        {
            get
            {
                if (_configuration == null)
                {
                    throw new Exception("Please initialize SessionFactory by calling:" +
                                        " InitSessionFactory.InitFactory() during program initialization once.");
                }
                return _configuration;
            }
            private set { _configuration = value; }
        }

        private static ISessionFactory _sessionFactory;

        public static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    throw new Exception("Please initialize SessionFactory by calling:" +
                                        " InitSessionFactory.InitFactory() during program initialization once.");
                }
                return _sessionFactory;
            }
        }

        public static void InitFactory(NhInitParams obj)
        {
            if (_sessionFactory != null)
            {
                return;
            }

            NhInitParams = obj;

            //LoggingSetup.SetupLogging();

            NhConfiguration = ConfigurationSetup.ConfigureNHibernate(obj);

            MappingSetup.Setup(NhConfiguration);

            //ValidationSetup.Setup(configuration);

            NhCachingSetup.Setup(NhConfiguration);

            //AudtingSetup.Setup(configuration);

            _sessionFactory = NhConfiguration.BuildSessionFactory();

            if (obj.DbType == ConfiguredDbTypes.SqLite)
            {
                var session = _sessionFactory.OpenSession();
                CurrentSessionContext.Bind(session);
                var schema = new SchemaExport(NhConfiguration);
                schema.Execute(false, true, false, session.Connection, null);
            }
        }
    }

}

