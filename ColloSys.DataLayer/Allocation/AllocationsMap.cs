using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.SharedDomain;

namespace ColloSys.DataLayer.Mapping
{
    public class AllocationsMap : EntityMap<Allocations>
    {
        public AllocationsMap()
        {
            Property(x => x.IsAllocated);
            Property(x => x.Bucket);
            Property(x => x.AmountDue);
            Property(x => x.ChangeReason, map => map.NotNullable(false));
            Property(x => x.AllocStatus);
            Property(x => x.NoAllocResons);
            Property(x => x.StartDate);
            Property(x => x.EndDate);
            Property(x => x.WithTelecalling);

            Property(p => p.Status);
            Property(p => p.Description, map => map.NotNullable(false));
            Property(p => p.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(p => p.OrigEntityId);
            Property(p => p.RowStatus);

            ManyToOne(x => x.AllocPolicy, map => map.NotNullable(false));
            ManyToOne(x => x.AllocSubpolicy, map => map.NotNullable(false));
            ManyToOne(x => x.Stakeholder, map => map.NotNullable(false));
            ManyToOne(x => x.Info, map => map.NotNullable(true));
        }
    }
}