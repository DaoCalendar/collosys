#region Reference

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class BillingRelationMap : EntityMap<BillingRelation>
    {
        public BillingRelationMap()
        {
            Table("BILLING_RELATION");

            #region Property
            Property(x => x.Priority);
            #endregion

            #region IDateRange
            Property(x => x.StartDate);
            Property(x => x.EndDate);
            #endregion

            #region IApprov

            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.Status);
            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);

            #endregion

            #region Many2Many

            ManyToOne(x => x.BillingPolicy, map =>
                {
                    map.NotNullable(true);
                    map.UniqueKey("UQ_BILLING_RELATION");
                });

            ManyToOne(x => x.BillingSubpolicy,
                    map =>
                    {   map.NotNullable(true);
                        map.UniqueKey("UQ_BILLING_RELATION");
                     });
            #endregion
           
        }
    }
}