using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhWorkingMap : EntityMap<StkhWorking>
    {
        public StkhWorkingMap()
        {
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
            Property(x => x.StartDate);
            Property(x => x.EndDate);
            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.Status);

            ManyToOne(x => x.Stakeholder);
            ManyToOne(x => x.GPincode);
        }
    }
}