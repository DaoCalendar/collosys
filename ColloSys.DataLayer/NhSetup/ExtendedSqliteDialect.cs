using System.Data;
using NHibernate.Dialect;

namespace ColloSys.DataLayer.NhSetup
{
    public class ExtendedSqliteDialect :SQLiteDialect
    {
        public ExtendedSqliteDialect()
        {
            RegisterColumnType(DbType.DateTime2, "DATETIME");
            RegisterColumnType(DbType.UInt16, "INT");
            RegisterColumnType(DbType.UInt32, "BIGINT");
            RegisterColumnType(DbType.UInt64, "DECIMAL(28)");
        }
    }
}
