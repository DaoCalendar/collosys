#region references

using System;
using NHibernate;
using NHibernate.Cfg;

#endregion

namespace ColloSys.DataLayer.NhSetup
{
    public static class SessionFactoryManager
    {
        private static Configuration _configuration;

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
            private set
            {
                _configuration = value;
            }
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
            //LoggingSetup.SetupLogging();

            NhConfiguration = Build(obj);

            _sessionFactory = NhConfiguration.BuildSessionFactory();
        }

        private static Configuration Build(NhInitParams obj)
        {
            var configuration = ConfigurationSetup.ConfigureNHibernate(obj);

            MappingSetup.Setup(configuration);

            //ValidationSetup.Setup(configuration);

            NhCachingSetup.Setup(configuration);

            //AudtingSetup.Setup(configuration);

            return configuration;
        }
    }
}


//            configuration.SetProperty("use_sql_comments", "false");

