#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;

#endregion

namespace ColloSys.DataLayer.Mapping
{

    public class PaymentMap : EntityMap<Payment>
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
            Property(x => x.ChargeOffMonth);
            Property(x => x.SHORT_NAME);
            Property(x => x.PROV_CHG_OF);
            Property(x => x.INT_IN_SUS);
            Property(x => x.CHARGE_OFF);
            Property(x => x.PRODUCT);
            Property(x => x.STATE);

            #region RLS Payment_PropertiesMapping
            Property(x => x.CostCenter);
            Property(x => x.NAME);
            Property(x => x.DISBURSEMENTDATE);
            Property(x => x.FINALINSTDT);
            Property(x => x.CYCLEDUEDT);
            Property(x => x.APPBR);
            Property(x => x.DTCHGOFF);
            Property(x => x.BOUNECHARGES);
            Property(x => x.LTCHGWAIVED723);
            Property(x => x.AMTUTLPROV567);
            Property(x => x.AMTUTLINTTSUS797);
            Property(x => x.ANYOTHERDUES799);
            Property(x => x.SNo);
            Property(x => x.Product);
            Property(x => x.Product_7);
            Property(x => x.Account);
            Property(x => x.Deptid);
            Property(x => x.OperUnit);
            Property(x => x.CClass);
            Property(x => x.Unit);
            #endregion
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




