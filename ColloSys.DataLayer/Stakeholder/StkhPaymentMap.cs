#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;
using NHibernate.Mapping.ByCode;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class StkhPaymentMap : EntityMap<StkhPayment>
    {
        public StkhPaymentMap()
        {
            Table("STKH_PAYMENT");

            #region properties
            Property(x => x.Products);
            Property(x => x.BankAccNo, map => map.NotNullable(false));
            Property(x => x.BankAccName, map => map.NotNullable(false));
            Property(x => x.BankIfscCode, map => map.NotNullable(false));
            Property(x => x.MobileElig);
            Property(x => x.TravelElig);
            //Property(x => x.VariableLiner, map => map.NotNullable(false));
            //Property(x => x.VariableWriteoff, map => map.NotNullable(false));
            Property(x => x.FixpayBasic);
            Property(x => x.FixpayHra);
            Property(x => x.FixpayOther);
            Property(x => x.FixpayTotal);
            Property(x => x.ServiceCharge);

            #endregion

            #region DateRange

            Property(x => x.StartDate);
            Property(x => x.EndDate);

            #endregion

            #region Approve Component

            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.Status);
            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);

            #endregion

            #region Relationships - ManyToOne

            ManyToOne(x => x.Stakeholder, map => map.NotNullable(true));
            ManyToOne(x => x.CollectionBillingPolicy);
            ManyToOne(x => x.RecoveryBillingPolicy);
            Bag(x => x.StkhWorkings, colmap => { }, map => map.OneToMany(x => { }));

            #endregion
        }
    }
}