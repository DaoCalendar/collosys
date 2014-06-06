#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class CacsActivityMap : EntityMap<CacsActivity>
    {
        public CacsActivityMap()
        {
            Property(x => x.TelecallerId, map =>
            {
                //map.UniqueKey("UK_CACS_ACTIVITY");
                map.Index("IX_CACS_ACTIVITY");
            });
            Property(x => x.AccountNo, map =>
            {
                //map.UniqueKey("UK_CACS_ACTIVITY");
                map.Index("IX_CACS_ACTIVITY");
            });
            Property(x => x.Products);
            Property(x => x.Region);
            Property(x => x.CallDateTime, map =>
            {
                //map.UniqueKey("UK_CACS_ACTIVITY");
                map.Index("IX_CACS_ACTIVITY");
            });
            Property(x => x.CallDuration);
            Property(x => x.CallDirection);
            Property(x => x.CallLocation);
            Property(x => x.CallResponce);
            Property(x => x.ActivityCode);
            Property(x => x.Ptp1Date);
            Property(x => x.Ptp1Amt);
            Property(x => x.Ptp2Date);
            Property(x => x.Ptp2Amt);
            Property(x => x.ExcuseCode);
            Property(x => x.MissingBasicInfo);
            Property(x => x.ConsiderInAllocation);
            Property(x => x.FileDate);
            ManyToOne(x => x.FileScheduler);
            Property(x => x.FileRowNo);
        }
    }
}
