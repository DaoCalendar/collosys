#region references

using System.Data;
using NHibernate.Driver;
using NHibernate.SqlTypes;

#endregion

namespace ColloSys.DataLayer.NhSetup
{
    public class ExtendedSql2008ClientDriver :  Sql2008ClientDriver  
    {
        protected override void InitializeParameter(IDbDataParameter dbParam, string name, SqlType sqlType)
        {
            if (Equals(sqlType, SqlTypeFactory.UInt16)) sqlType = SqlTypeFactory.Int32;
            if (Equals(sqlType, SqlTypeFactory.UInt32)) sqlType = SqlTypeFactory.Int64;
            if (Equals(sqlType, SqlTypeFactory.UInt64)) sqlType = SqlTypeFactory.Decimal;
            base.InitializeParameter(dbParam, name, sqlType);
        }
    }
}