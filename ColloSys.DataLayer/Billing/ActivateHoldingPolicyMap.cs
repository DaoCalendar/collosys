using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Mapping
{
    public class ActivateHoldingPolicyMap : EntityMap<ActivateHoldingPolicy>
    {
        public ActivateHoldingPolicyMap()
        {
            Property(x=>x.Products);

            ManyToOne(x=>x.HoldingPolicy,map=>map.NotNullable(true));

            ManyToOne(x=>x.Stakeholder,map=>map.NotNullable(true));
        }
    }
}