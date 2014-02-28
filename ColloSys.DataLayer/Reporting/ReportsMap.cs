#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class ReportsMap : EntityMap<Reports>
    {
        public ReportsMap()
        {
            Table("REPORTS");
            Property(x => x.Name);
            Property(x => x.Filter, map =>
                {
                    map.NotNullable(false);
                    map.Length(4001);
                });
            Property(x => x.TableName);
            Property(x => x.Columns, map => map.Length(4001));
            Property(x => x.ColumnsFilter, map => map.Length(4001));
        }
    }
}