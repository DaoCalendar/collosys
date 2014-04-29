using System.Configuration;
using System.IO;
using ColloSys.DataLayer.NhSetup;
using ColloSys.UserInterface.Areas.Developer.Models.Excel2Db;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.Shared.ConfigSectionReader;
using NUnit.Framework;

namespace ReflectionExtension.Tests
{
    [SetUpFixture]
   public class SetUpAssemblies
    {

        protected readonly FileStream FileStream;
        protected readonly FileInfo FileInfo;
        private static ConnectionStringSettings _connectionString;
        private static int _initCount;
        private static ConfiguredDbTypes _dbType;

        public SetUpAssemblies()
        {
            FileStream = ResourceReader.GetEmbeddedResourceAsFileStream("DrillDown_Txn_1.xls");
            FileInfo=new FileInfo(FileStream.Name);
            InitNhibernate();
        }

        public void InitNhibernate()
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
