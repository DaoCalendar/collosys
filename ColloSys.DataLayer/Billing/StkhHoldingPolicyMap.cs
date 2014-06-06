using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    public class StkhHoldingPolicyMap : EntityMap<StkhHoldingPolicy>
    {
        public StkhHoldingPolicyMap()
        {
            Property(x => x.Products);
            Property(x => x.StartMonth);
            ManyToOne(x => x.HoldingPolicy, map => map.NotNullable(true));
            ManyToOne(x => x.Stakeholder, map => map.NotNullable(true));
        }
    }
}