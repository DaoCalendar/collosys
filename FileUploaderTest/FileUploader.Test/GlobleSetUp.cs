using System.Configuration;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.NhSetup;
using ColloSys.Shared.ConfigSectionReader;
using NUnit.Framework;

namespace ReflectionExtension.Tests.FileUploader.Test
{
    [SetUpFixture]
    public class GlobalSetupAsm
    {
        private static ConnectionStringSettings _connectionString;
        private static int _initCount;
        private static ConfiguredDbTypes _dbType;

        #region Setup

        [SetUp]
        public void SetUp()
        {
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

        #endregion
    }
}
