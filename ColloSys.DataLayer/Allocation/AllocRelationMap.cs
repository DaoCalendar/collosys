#region references

using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class AllocRelationMap : EntityMap<AllocRelation>
    {
        public AllocRelationMap()
        {
            Table("ALLOC_RELATION");
            Property(x => x.Priority, map => map.NotNullable(true));

            Property(x => x.StartDate, map => map.NotNullable(true));
            Property(x => x.EndDate);

            Property(p => p.Status);
            Property(p => p.Description, map => map.NotNullable(false));
            Property(p => p.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);

            #region relationships - many2one

            ManyToOne(x => x.AllocPolicy, map =>
                {
                    map.NotNullable(true);
                    //map.UniqueKey("UQ_ALLOCATION_RELATION");
                });

            ManyToOne(x => x.AllocSubpolicy, map =>
                {
                    map.NotNullable(true);
                    //map.UniqueKey("UQ_ALLOCATION_RELATION");
                });

            #endregion

        }
    }
}