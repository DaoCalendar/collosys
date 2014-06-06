using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Generic
{
    public class ProductConfigMap : EntityMap<ProductConfig>
    {
        public ProductConfigMap()
        {
            Property(x => x.Product, map => map.UniqueKey("UK_PRODUCT_PRODUCTNAME"));
            Property(x => x.ProductGroup);
            Property(x => x.AllocationResetStrategy);
            Property(x => x.BillingStrategy);
            Property(x => x.CycleCodes);
            Property(x => x.HasTelecalling);
            Property(x => x.FrCutOffDaysCycle);
            Property(x => x.FrCutOffDaysMonth);
            Property(x=>x.HasWriteOff);
            Property(x=>x.LinerTable);
            Property(x=>x.WriteoffTable);
            Property(x => x.PaymentTable);
        }
    }
}
