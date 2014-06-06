#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;

#endregion

namespace ColloSys.DataLayer.Mapping
{

    public  class PaymentMap : EntityMap<Payment>
    {
        public PaymentMap()
        {
            ManyToOne(x => x.FileScheduler, map => map.NotNullable(false));

            Property(x => x.FileDate);
            Property(x => x.FileRowNo);
            Property(x => x.AccountNo, map => map.Index("IX_PAYMENT"));

            Property(x => x.TransCode);
            Property(x => x.TransDate);
            Property(x => x.TransAmount);
            Property(x => x.TransDesc, map => map.NotNullable(false));
            Property(x => x.IsDebit);
            Property(x => x.Products, map => map.NotNullable(false));


            Property(x => x.IsExcluded);
            Property(x => x.ExcludeReason, map => map.NotNullable(false));
            Property(x => x.BillDate);
            Property(x => x.BillStatus);

            Property(x => x.Status);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
        }
    }

    //public class CPaymentMap : PaymentMap<CPayment>
    //{
    //    public CPaymentMap()
    //        : base("CCMS")
    //    {
    //        //Table("C_PAYMENT");
    //    }
    //}
}




