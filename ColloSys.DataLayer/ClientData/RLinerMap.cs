#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class RLinerMap : EntityMap<RLiner>
    {
        public RLinerMap()
        {
            Table("R_LINER");

            #region Relationship Mapping

            Set(x => x.RAllocs, colmap => { }, map => map.OneToMany(x => { }));
            ManyToOne(x => x.GPincode, map => { });
            ManyToOne(x => x.FileScheduler, map => map.NotNullable(true));

            #endregion

            #region Property Mapping

            Property(x => x.FileDate, map =>
            {
                map.UniqueKey("RLS_UQ_LINER");
                map.Index("RLS_IX_LINER");
            });

            Property(x => x.FileRowNo);
            Property(y => y.AccountNo, map =>
            {
                map.UniqueKey("RLS_UQ_LINER");
                map.Index("RLS_IX_LINER");
            });

            Property(x => x.Flag);
            Property(x => x.PrincipalDue);
            Property(x => x.DelqHistoryString);
            Property(x => x.CustomerName, map => map.NotNullable(false));
            Property(x => x.Branch, map => map.NotNullable(false));
            Property(x => x.Product);
            Property(x => x.ProductName);
            Property(x => x.Cycle);
            Property(x => x.AgeCode);
            Property(x => x.Bucket);
            Property(x => x.IsImpaired);
            Property(x => x.LoanTotalDue);
            Property(x => x.LoanPrinDue);
            Property(x => x.Emi);
            Property(x => x.EmiDue);
            Property(x => x.TotalDue);
            Property(x => x.RevisedDue);
            Property(p => p.BounceCharge);
            Property(p => p.FeeCharge);
            Property(p => p.InterestCharge);
            Property(p => p.LateCharge);
            Property(x => x.Tenure);
            Property(x => x.FirstInstDate);
            Property(x => x.FinalInstDate);
            Property(x => x.IsReferred);
            Property(x => x.Pincode);
            Property(x => x.NoAllocResons);
            Property(x => x.CustStatus, map => map.NotNullable(false));
            Property(x => x.AllocStatus);

            #endregion
        }
    }
}



//Property(x => x.InterestPct);
//IPincode
//Property(x => x.Pincode);
//Property(x => x.DoAllocate);

//Property(p => p.DelqAmount);
//Property(p => p.DelqDate);
//Property(p => p.DelqStatus);

