using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhPaymentMap : EntityMap<StkhPayment>
    {
        public StkhPaymentMap()
        {
            Property(x => x.MobileElig);
            Property(x => x.TravelElig);
            Property(x => x.FixpayBasic);
            Property(x => x.FixpayGross);
            Property(x => x.StartDate);
            Property(x => x.EndDate);

            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.ApprovalStatus);

            ManyToOne(x => x.Stakeholder, map => map.NotNullable(true));
        }
    }
}