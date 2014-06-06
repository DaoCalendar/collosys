#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class ELinerMap : EntityMap<ELiner>
    {
        public ELinerMap()
        {

            #region Relationship Mapping

            Set(x => x.Allocs, colmap => { }, map => map.OneToMany(x => { }));
            ManyToOne(x => x.FileScheduler, map => map.NotNullable(true));
            ManyToOne(x => x.GPincode, map => { });

            #endregion

            #region Property Mapping
            Property(x => x.FileDate, map =>
            {
                map.UniqueKey("EBBS_UQ_LINER");
                map.Index("EBBS_IX_LINER");
            });

            Property(x => x.FileRowNo);
            Property(x => x.AccountNo, map =>
            {
                map.UniqueKey("EBBS_UQ_LINER");
                map.Index("EBBS_IX_LINER");
            });

            Property(x => x.CustomerName, map => map.NotNullable(false));
            Property(x => x.Product);
            Property(x => x.ProductCode);
            Property(x => x.OdLimit);

            Property(x => x.TotalDue);
            Property(x => x.AmountRepaid);
            Property(x => x.CurrentDue);
            Property(x => x.MinimumDue);
            Property(x => x.IsReferred);
            Property(x => x.Cycle);
            Property(x => x.DayPastDue);
            Property(x => x.Bucket);
            Property(x => x.BucketDue);
            Property(x => x.Bucket1Due);
            Property(x => x.Bucket2Due);
            Property(x => x.Bucket3Due);
            Property(x => x.Bucket4Due);
            Property(x => x.Bucket5Due);

            Property(x => x.InterestPct);
            Property(x => x.InterestCharge);
            Property(x => x.FeeCharge);

            Property(x => x.AccountOpenDate);
            Property(x => x.ExpirtyDate);
            Property(x => x.Flag);
            Property(x => x.DelqHistoryString);
            Property(x => x.Pincode);
            Property(x => x.PeakBucket);
            Property(x => x.IsSetteled);
            Property(x => x.AccountStatus);
            Property(x=>x.Pincode);
            Property(x => x.NoAllocResons);

            Property(x => x.CustStatus, map => map.NotNullable(false));
            Property(x => x.AllocStatus);

            #endregion

        }
    }
}


