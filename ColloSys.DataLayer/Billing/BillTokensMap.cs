using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    public class BillTokensMap : EntityMap<BillTokens>
    {
        public BillTokensMap()
        {
            Property(x => x.Type);
            Property(x => x.Text);
            Property(x => x.Value);
            Property(x => x.DataType);
            Property(x=>x.GroupType);
            Property(x => x.GroupId);
            Property(x => x.Priority);

            ManyToOne(x => x.BillingSubpolicy, map => map.NotNullable(false));
        }
    }
}