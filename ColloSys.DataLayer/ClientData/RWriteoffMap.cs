#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class RWriteoffMap : EntityMap<RWriteoff>
    {
        public RWriteoffMap()
        {
            Table("R_WRITEOFF");

            #region Relationship Mapping

            Set(x => x.Allocs, colmap => { }, map => map.OneToMany(x => { }));
            ManyToOne(x => x.FileScheduler, map => map.NotNullable(true));
            ManyToOne(x => x.GPincode, map => { });

            #endregion

            #region Property Mapping

            Property(x => x.FileDate, map =>
            {
                map.UniqueKey("RLS_UQ_WRITEOFF");
                map.Index("RLS_IX_WRITEOFF");
            });

            Property(x => x.FileRowNo);
            Property(y => y.AccountNo, map =>
            {
                map.UniqueKey("RLS_UQ_WRITEOFF");
                map.Index("RLS_IX_WRITEOFF");
            });

            Property(x => x.CustomerName, map => map.NotNullable(false));
            Property(x => x.Product);
            Property(x => x.ProductName, map => map.NotNullable(false));
            Property(x => x.Branch, map => map.NotNullable(false));
            Property(x => x.ChargeOffDate);
            Property(x => x.TotalDue);
            Property(x => x.PrincipalDue);
            Property(x => x.Recovery);
            Property(x => x.CurrentDue);
            Property(x => x.IsSetteled);
            Property(x => x.Comment, map => map.NotNullable(false));
            Property(p => p.BounceCharge);
            Property(p => p.FeeCharge);
            Property(p => p.InterestCharge);
            Property(p => p.LateCharge);
            Property(x => x.Cycle);
            Property(x => x.DisbursementDate);
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



