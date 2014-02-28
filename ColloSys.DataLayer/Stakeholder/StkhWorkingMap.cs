#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class StkhWorkingMap : EntityMap<StkhWorking>
    {
        public StkhWorkingMap()
        {
            Table("STKH_WORKING");

            #region properties

            Property(x => x.BucketStart);
            Property(x=>x.ReportsTo);
            Property(x => x.BucketEnd);
            Property(x => x.Country);
            Property(x => x.Region);
            Property(x => x.State);
            Property(x => x.Cluster);
            Property(x => x.District);
            Property(x => x.City);
            Property(x => x.Area);
            Property(x => x.Products);
            Property(x=>x.LocationLevel);

            #endregion

            #region DateRange
            Property(x => x.StartDate);

            Property(x => x.EndDate);

            #endregion

            #region Approve
            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.Status);
            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);
            #endregion

            #region Relationships - ManyToOne
            ManyToOne(x => x.Stakeholder);
            //ManyToOne(x => x.ReportsTo);
            ManyToOne(x => x.GPincode);
            ManyToOne(x => x.StkhPayment);
            #endregion
        }
    }
}