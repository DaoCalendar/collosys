using System.Configuration;
using System.IO;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.NhSetup;
using ColloSys.Shared.ConfigSectionReader;
using ColloSys.UserInterface.Areas.Developer.Models.Excel2Db;
using NUnit.Framework;

namespace ReflectionExtension.Tests
{
     [SetUpFixture]
    class SetUpAssembliesForEbbs
    {
        
        protected readonly FileStream FileStream;
        protected readonly FileInfo FileInfo;
        private static ConnectionStringSettings _connectionString;
        private static int _initCount;
        private static ConfiguredDbTypes _dbType;

        public SetUpAssembliesForEbbs()
        {
            FileStream = ResourceReader.GetEmbeddedResourceAsFileStream("AEB Auto Charge Off Base - 28.01.2014.xls");
            FileInfo=new FileInfo(FileStream.Name);
            InitNhibernate();
        }

        private void InitNhibernate()
        {
            if (_initCount++ != 0) return;
            _dbType = ConfiguredDbTypes.MsSql;
            _connectionString = ColloSysParam.WebParams.ConnectionString;
            var obj = new NhInitParams { ConnectionString = _connectionString, DbType = _dbType, IsWeb = false };

            SessionManager.InitNhibernate(obj);
            SessionManager.BindNewSession();
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
    

