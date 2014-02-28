#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;
#endregion

namespace ColloSys.DataLayer.Mapping
{
    //public class EPaymentMap : EntityMap<EPayment>
    //{
    //    public EPaymentMap()
    //    {
    //        Table("E_PAYMENT");
    //        ManyToOne(x => x.FileScheduler, map => map.NotNullable(true));

    //        Property(x => x.FileDate, map => map.Index("EBBS_IX_PAYMENT"));
    //        Property(x => x.FileRowNo);
    //        Property(x => x.AccountNo, map => map.Index("EBBS_IX_PAYMENT"));

    //        Property(p => p.TransCode);
    //        Property(p => p.TransDate);
    //        Property(p => p.TransDesc, map => map.NotNullable(false));
    //        Property(p => p.TransAmount);
    //        Property(p => p.IsDebit);

    //        Property(p => p.IsExcluded);
    //        Property(p => p.ExcludeReason, map => map.NotNullable(false));
    //        Property(p => p.BillDate);
    //        Property(p => p.BillStatus);

    //        Property(p => p.Status);
    //        Property(p => p.Description, map => map.NotNullable(false));
    //        Property(p => p.ApprovedBy, map => map.NotNullable(false));
    //        Property(p => p.ApprovedOn);
    //    }

    public class EPaymentMap : PaymentMap<EPayment>
    {
        public EPaymentMap()
            : base("EBBS")
        {
            //Table("E_PAYMENT");
        }
    }
}

