using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    public class BillingSubpolicyMap : EntityMap<BillingSubpolicy>
    {
        public BillingSubpolicyMap()
        {
            Property(x => x.Name, map => map.UniqueKey("UQ_BILLSUBPOLICY_NAME"));
            Property(x => x.IsActive);
            Property(x => x.IsInUse);
            Property(x => x.Products, map => map.UniqueKey("UQ_BILLSUBPOLICY_NAME"));
            Property(x => x.PayoutSubpolicyType);
            Property(x => x.OutputType);
            Property(x => x.GroupBy, map => map.NotNullable(false));
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.PolicyType);
        
            Bag(x => x.BillingRelations, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.BillDetails, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.BillTokens, colmap => { }, map => map.OneToMany(x => { }));
        }
    }
}

