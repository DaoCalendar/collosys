using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    public class BillSummaryMap : EntityMap<BillSummary>
    {
        public BillSummaryMap()
        {
            ManyToOne(x => x.Stakeholder, map => { map.NotNullable(true); map.UniqueKey("UQ_BILLING_AMOUNT"); });
            Property(x => x.Products, map => map.UniqueKey("UQ_BILLING_AMOUNT"));
            Property(x => x.BillMonth, map => map.UniqueKey("UQ_BILLING_AMOUNT"));
            Property(x => x.Cycle, map => map.UniqueKey("UQ_BILLING_AMOUNT"));
            Property(x => x.OriginMonth, map => map.UniqueKey("UQ_BILLING_AMOUNT"));

            Property(x => x.FixedAmount);
            Property(x => x.VariableAmount);
            Property(x => x.Deductions);
            Property(x => x.CappingDeduction);
            Property(x => x.ProcFeeDeduction);
            Property(x => x.TaxAmount);
            Property(x => x.TotalAmount);
            Property(x => x.PayStatus);
            Property(x => x.PayStatusDate);
            Property(x => x.PayStatusHistory);

            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.Status);
        }
    }
}