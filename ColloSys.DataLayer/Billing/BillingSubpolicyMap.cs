#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class BillingSubpolicyMap : EntityMap<BillingSubpolicy>
    {
        public BillingSubpolicyMap()
        {
            Table("BILLING_SUBPOLICY");

            #region property

            Property(x => x.Name, map => map.UniqueKey("UQ_BILLSUBPOLICY_NAME"));
            Property(x => x.IsBasic);
            Property(x => x.IsActive);
            Property(x => x.IsInUse);

            Property(x => x.Products, map => map.UniqueKey("UQ_BILLSUBPOLICY_NAME"));

            Property(x => x.Category, map => map.UniqueKey("UQ_BILLSUBPOLICY_NAME"));

            Property(x => x.PayoutSubpolicyType, map => map.UniqueKey("UQ_BILLSUBPOLICY_NAME"));

            Property(x => x.OutputType);
            Property(x => x.GroupBy, map => map.NotNullable(false));
            Property(x => x.Description, map => map.NotNullable(false));

            Property(x => x.ProcessingFee);
            Property(x => x.PayoutCapping);
            #endregion

            #region Bags-relationship


            Set(x => x.BillingRelations,
                colmap => { }, map => map.OneToMany(x => { }));
            Set(x => x.BConditions,
                colmap => { }, map => map.OneToMany(x => { }));
            Set(x => x.BillDetails, colmap => { }, map => map.OneToMany(x => { }));

            //ManyToOne(x => x.BConditionPayoutId,
            //          map =>
            //              {
            //                  map.Column("PayoutId");
            //                  map.NotNullable(true);
            //              });
            //ManyToOne(x => x.BConditionConditionId,
            //          map => map.Column("ConditionId"));
            //Set(x => x.BOutputs,
            //    colmap => { }, map => map.OneToMany(x => { }));

            #endregion
        }
    }
}

