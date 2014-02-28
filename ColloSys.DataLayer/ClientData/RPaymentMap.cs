#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;
#endregion

namespace ColloSys.DataLayer.Mapping
{
    //public class RPaymentMap : EntityMap<RPayment>
    //{
        //public RPaymentMap()
        //{
        //    Table("R_PAYMENT");
        //    ManyToOne(x => x.FileScheduler, map => map.NotNullable(true));

        //    Property(x => x.FileDate, map => map.Index("RLS_IX_PAYMENT"));
        //    Property(x => x.FileRowNo);
        //    Property(x => x.AccountNo, map => map.Index("RLS_IX_PAYMENT"));

        //    Property(x => x.TransCode);
        //    Property(x => x.TransDate);
        //    Property(x => x.TransAmount);
        //    Property(x => x.TransDesc, map => map.NotNullable(false));
        //    Property(x => x.IsDebit);
            
        //    Property(x => x.IsExcluded);
        //    Property(x => x.ExcludeReason, map => map.NotNullable(false));
        //    Property(x => x.BillDate);
        //    Property(x => x.BillStatus);
            
        //    Property(x => x.Status);
        //    Property(x => x.Description, map => map.NotNullable(false));
        //    Property(x => x.ApprovedBy, map => map.NotNullable(false));
        //    Property(p => p.ApprovedOn);
        //}

    //}

    public class RPaymentMap : PaymentMap<RPayment>
    {
        public RPaymentMap()
            : base("RLS")
        {
            //Table("R_PAYMENT");
        }
    }

}


//#region Product Mapping
//Property(x => x.Product, map => map.NotNullable(true));
//#endregion

