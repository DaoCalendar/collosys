#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

#endregion

namespace ColloSys.DataLayer.Mapping
{

    public  class PaymentMap<T> : EntityMap<T> where T : Payment
    {
        protected PaymentMap(string cat)
        {
            Table(cat[0] + "_PAYMENT");
            ManyToOne(x => x.FileScheduler, map => map.NotNullable(false));

            Property(x => x.FileDate, map => map.Index(cat + "_IX_PAYMENT"));
            Property(x => x.FileRowNo);
            Property(x => x.AccountNo, map => map.Index(cat + "_IX_PAYMENT"));

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




