using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    public class BillAdhocMap : EntityMap<BillAdhoc>
    {
        public BillAdhocMap()
        {
            Property(x => x.IsRecurring);
            Property(x => x.TotalAmount);
            Property(x => x.RemainingAmount);
            Property(x => x.StartMonth);
            Property(x => x.EndMonth);
            Property(x => x.Tenure);
            Property(x => x.IsCredit);
            Property(x => x.IsPretax);
            Property(x => x.ReasonCode, map => map.NotNullable(false));
            Property(x => x.Products);

            ManyToOne(x => x.Stakeholder, map => map.NotNullable(true));
            Bag(x => x.BillDetails, colmap => { }, map => map.OneToMany(x => { }));
        }
    }
}
