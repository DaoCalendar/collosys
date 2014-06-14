using ColloSys.DataLayer.NhSetup;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.Shared.ConfigSectionReader;
using NUnit.Framework;

namespace ReflectionExtension.Tests
{
    [SetUpFixture]
    public sealed class SetUpAssemblies
    {
        public SetUpAssemblies()
        {
            InitNhibernate();
        }

        private void InitNhibernate()
        {

            var connectionString = ColloSysParam.WebParams.ConnectionString;
            var obj = new NhInitParams
            {
                ConnectionString = connectionString,
                DbType = ConfiguredDbTypes.MsSql,
                IsWeb = false
            };

            SessionManager.InitNhibernate(obj);
        }
    }
}
