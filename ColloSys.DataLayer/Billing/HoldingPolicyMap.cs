using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Mapping
{
    public class HoldingPolicyMap : EntityMap<HoldingPolicy>
    {
        public HoldingPolicyMap()
        {
            Property(x=>x.Name);
            Property(x=>x.Description);
            Property(x=>x.StartDate);
            Property(x=>x.EndDate);
            Property(x=>x.ApplyOn);
            Property(x=>x.Products);
            Property(x=>x.Rule);
            Property(x=>x.Value);
            Property(x=>x.TransactionType);
            
            Property(x=>x.Tenure);

            Bag(x => x.ActivateHoldingPolicies,
                colmap => { }, map => map.OneToMany(x => { }));
        }
    }
}