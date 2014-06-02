#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class BillDetailMap : EntityMap<BillDetail>
    {
        public BillDetailMap()
        {
            Table("BILL_DETAILS");

            #region property
            Property(x => x.BillMonth);
            Property(x => x.BillCycle);
            Property(x => x.Amount);
            Property(x => x.TraceLog, map => map.Length(4001));

            //Property(x => x.ElementType);
            //Property(x => x.ElementId);
            //Property(x => x.ParamName);
            //Property(x => x.ParamValue);
            #endregion

            #region component

            Property(x => x.Products);
            Property(x => x.PaymentSource);

            #endregion

            #region relationship
            ManyToOne(x => x.Stakeholder, map => map.NotNullable(true));
            ManyToOne(x => x.BillingPolicy, map => map.NotNullable(false));
            ManyToOne(x => x.BillingSubpolicy, map => map.NotNullable(false));
            ManyToOne(x => x.BillAdhoc, map => map.NotNullable(false));
            #endregion

            Bag(x => x.CustBillViewModels, colmap => { }, map => map.OneToMany(x => { }));

            Property(x => x.PolicyType);
            Property(x => x.OriginMonth);
            Property(x => x.BaseAmount);
        }
    }
}