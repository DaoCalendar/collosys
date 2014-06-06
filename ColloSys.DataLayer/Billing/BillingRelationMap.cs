using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    public class BillingRelationMap : EntityMap<BillingRelation>
    {
        public BillingRelationMap()
        {
            ManyToOne(x => x.BillingPolicy, map => map.NotNullable(true));
            ManyToOne(x => x.BillingSubpolicy, map => map.NotNullable(true));
            Property(x => x.Priority);
            Property(x => x.StartDate);
            Property(x => x.EndDate);

            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.Status);
            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);
        }
    }
}