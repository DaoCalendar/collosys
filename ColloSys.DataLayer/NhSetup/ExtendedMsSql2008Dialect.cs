#region References
using System.Data;
using NHibernate.Dialect;
#endregion

namespace ColloSys.DataLayer.NhSetup
{
    public class ExtendedMsSql2008Dialect : MsSql2008Dialect
    {
        public ExtendedMsSql2008Dialect()
        {
            // Tell NH that we can handle the ADO DbTypes
            RegisterColumnType(DbType.UInt16, "INT");
            RegisterColumnType(DbType.UInt32, "BIGINT");
            RegisterColumnType(DbType.UInt64, "DECIMAL(28)");
        }
    }
}