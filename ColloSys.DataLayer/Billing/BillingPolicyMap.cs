using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    public class BillingPolicyMap : EntityMap<BillingPolicy>
    {
        public BillingPolicyMap()
        {
            Property(x => x.Name);
            Property(x => x.Products);
            Property(x => x.Category);
            Property(x => x.PolicyFor);
            Property(x => x.PolicyForId);
            Property(x => x.PolicyType);

            Bag(x => x.BillingRelations, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.BillTokens, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.BillDetails, colmap => { }, map => map.OneToMany(x => { }));
        }
    }
}