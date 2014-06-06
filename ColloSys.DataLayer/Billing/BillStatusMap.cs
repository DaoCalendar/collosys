using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    public class BillStatusMap : EntityMap<BillStatus>
    {
        public BillStatusMap()
        {
            Property(x => x.ExternalId);
            Property(x => x.BillMonth);
            Property(x => x.OriginMonth);
            ManyToOne(x => x.Stakeholder);
            Property(x => x.Products);
            Property(x => x.BillCycle);
            Property(x => x.Status);
        }
    }
}

