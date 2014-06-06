using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhPaymentMap : EntityMap<StkhPayment>
    {
        public StkhPaymentMap()
        {
            Property(x => x.Products);
            Property(x => x.MobileElig);
            Property(x => x.TravelElig);
            Property(x => x.FixpayBasic);
            Property(x => x.FixpayHra);
            Property(x => x.FixpayOther);
            Property(x => x.FixpayTotal);
            Property(x => x.ServiceCharge);

            Property(x => x.StartDate);
            Property(x => x.EndDate);

            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.Status);
            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);

            ManyToOne(x => x.Stakeholder, map => map.NotNullable(true));
        }
    }
}