#region references

using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class CLinerMap : EntityMap<CLiner>
    {
        public CLinerMap()
        {
            Table("C_LINER");
            Set(x => x.Allocs, colmap => { }, map => map.OneToMany(x => { }));
            ManyToOne(x => x.FileScheduler, map => map.NotNullable(true));

            Property(x => x.FileDate, map =>
            {
                map.UniqueKey("CCMS_UQ_LINER");
                map.Index("CCMS_IX_LINER");
            });

            Property(x => x.FileRowNo);
            Property(y => y.AccountNo, map =>
            {
                map.UniqueKey("CCMS_UQ_LINER");
                map.Index("CCMS_IX_LINER");
            });

            Property(x => x.GlobalCustId);
            Property(x => x.CustomerName, map => map.NotNullable(false));
            Property(x => x.CreditLimit);
            Property(x => x.Cycle);
            Property(x => x.Location);
            Property(x=>x.IsReferred);

            Property(x => x.Product);

            Property(x => x.CustTotalDue);
            Property(x => x.Bucket);
            Property(x => x.TotalDue);
            Property(x => x.OutStandingBalance);
            Property(x => x.CurrentBalance);
            Property(x => x.CurrentDue);
            Property(x => x.UnbilledDue);
            Property(x => x.Bucket0Due);
            Property(x => x.Bucket1Due);
            Property(x => x.Bucket2Due);
            Property(x => x.Bucket3Due);
            Property(x => x.Bucket4Due);
            Property(x => x.Bucket5Due);
            Property(x => x.BucketAmount);
            Property(x => x.LastPayAmount);
            Property(x => x.LastPayDate);
            Property(x => x.Block);
            Property(x => x.AltBlock);
            Property(x => x.Pincode);
            Property(x=>x.NoAllocResons);

            Property(x => x.Flag);
            Property(x => x.DelqHistoryString);
            Property(x => x.Pincode);
            Property(x => x.PeakBucket);
            Property(x => x.AccountStatus);

            Property(x => x.AllocStatus);
            Property(x => x.CustStatus, map => map.NotNullable(false));
            ManyToOne(x => x.GPincode, map => { });
        }
    }
}



            //#region IDateRange
            //Property(x => x.StartDate);
            //Property(x => x.EndDate);
            //Property(x => x.UnbilledAmount);
            //#endregion

            //#region IDelq
            //Property(x => x.CustTotalDue, map => map.NotNullable(true));
            //Property(x => x.Pincode);
            //Property(x => x.DoAllocate, map => map.NotNullable(true));
            //Property(p => p.DelqAmount, map => map.NotNullable(true));
            //Property(p => p.DelqDate, map => map.NotNullable(true));
            //Property(p => p.DelqStatus, map => map.NotNullable(true));
            //Property(x => x.InterestPct, map => map.NotNullable(true));
            //#endregion

