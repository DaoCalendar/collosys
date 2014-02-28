#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;
#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class BillingPolicyMap : EntityMap<BillingPolicy>
    {
        public BillingPolicyMap()
        {
            Table("BILLING_POLICY");

            #region properties

            Property(x => x.Name, map => map.UniqueKey("UQ_BILLING_POLICY"));

            Property(x => x.Products, map => map.UniqueKey("UQ_BILLING_POLICY"));

            Property(x => x.Category, map => map.UniqueKey("UQ_BILLING_POLICY"));

            #endregion

            #region IApprove

            Property(x => x.Status);
            Property(x => x.ApprovedBy, map => map.NotNullable(false));

            Property(p => p.ApprovedOn);

            Property(x => x.Description, map => map.NotNullable(false));

            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);

            #endregion

            #region Bags- relationships
            Set(x => x.BillingRelations, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.CollectionStkhPayments, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.RecoveryStkhPayments, colmap => { }, map => map.OneToMany(x => { }));
            Set(x => x.StkhPayments, colmap => { }, map => map.OneToMany(x => { }));
            Set(x => x.BillDetails, colmap => { }, map => map.OneToMany(x => { }));
            #endregion
        }
    }
}