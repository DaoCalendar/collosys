using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

namespace ColloSys.DataLayer.Billing
{
    public class BillDetailMap : EntityMap<BillDetail>
    {
        public BillDetailMap()
        {
            Property(x => x.BillMonth);
            Property(x => x.BillCycle);
            Property(x => x.Amount);
            Property(x => x.TraceLog, map => map.Length(4001));
            Property(x => x.Products);
            Property(x => x.PaymentSource);
            Property(x => x.PolicyType);
            Property(x => x.OriginMonth);
            Property(x => x.BaseAmount);

            ManyToOne(x => x.Stakeholder, map => map.NotNullable(true));
            ManyToOne(x => x.BillingPolicy, map => map.NotNullable(false));
            ManyToOne(x => x.BillingSubpolicy, map => map.NotNullable(false));
            ManyToOne(x => x.BillAdhoc, map => map.NotNullable(false));

            Bag(x => x.CustBillViewModels, colmap => { }, map => map.OneToMany(x => { }));

        }
    }
}