#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;
#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class BillAmountMap : EntityMap<BillAmount>
    {
        public BillAmountMap()
        {
            Table("BILL_AMOUNT");

            #region property
            Property(x => x.Products, map => map.UniqueKey("UQ_BILLING_AMOUNT"));
            Property(x => x.BillMonth, map => map.UniqueKey("UQ_BILLING_AMOUNT"));
            Property(x => x.Cycle, map => map.UniqueKey("UQ_BILLING_AMOUNT"));
            Property(x => x.FixedAmount);
            Property(x => x.VariableAmount);
            Property(x => x.Deductions);
            Property(x => x.TaxAmount);
            Property(x => x.PayStatus);
            Property(x=>x.PayStatusDate);
            Property(x=>x.PayStatusHistory);
            Property(x=>x.CappingDeduction);
            Property(x=>x.ProcFeeDeduction);
            Property(x=>x.TotalAmount);
            #endregion

            #region IDateRange

            Property(x => x.StartDate);

            Property(x => x.EndDate);

            #endregion

            #region IApprove
            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.Status);
            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);

            #endregion

            #region relationship

            ManyToOne(x => x.Stakeholder, map =>
                {
                    map.NotNullable(true);
                    map.UniqueKey("UQ_BILLING_AMOUNT");
                });

            #endregion

            Property(x => x.OriginMonth, map => map.UniqueKey("UQ_BILLING_AMOUNT"));
        }
    }
}