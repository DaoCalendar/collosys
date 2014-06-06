using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Mapping
{
    public class AllocRelationMap : EntityMap<AllocRelation>
    {
        public AllocRelationMap()
        {
            Property(x => x.Priority);
            Property(x => x.StartDate);
            Property(x => x.EndDate);

            Property(p => p.Status);
            Property(p => p.Description, map => map.NotNullable(false));
            Property(p => p.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);

            ManyToOne(x => x.AllocPolicy, map => map.NotNullable(true));
            ManyToOne(x => x.AllocSubpolicy, map => map.NotNullable(true));
        }
    }
}