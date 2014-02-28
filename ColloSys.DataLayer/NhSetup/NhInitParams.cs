using System.Configuration;

namespace ColloSys.DataLayer.NhSetup
{
    public class NhInitParams
    {
        public NhInitParams()
        {
            DbType=ConfiguredDbTypes.MsSql;
            ConnectionString=new ConnectionStringSettings();
            IsWeb = false;
        }

        public ConfiguredDbTypes DbType { get; set; }

        public ConnectionStringSettings ConnectionString { get; set; }

        public bool IsWeb { get; set; }
    }
}
