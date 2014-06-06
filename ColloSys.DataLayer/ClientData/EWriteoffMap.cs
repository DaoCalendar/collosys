#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class EWriteoffMap : EntityMap<EWriteoff>
    {
        public EWriteoffMap()
        {
            #region Relationship Mapping

            Set(x => x.Allocs, colmap => { }, map => map.OneToMany(x => { }));
            ManyToOne(x => x.FileScheduler, map => map.NotNullable(true));
            ManyToOne(x => x.GPincode, map => { });

            #endregion

            #region Property Mapping

            Property(x => x.FileDate, map =>
            {
                map.UniqueKey("EBBS_UQ_WRITEOFF");
                map.Index("EBBS_IX_WRITEOFF");
            });
            Property(x => x.FileRowNo);
            Property(y => y.AccountNo, map =>
            {
                map.UniqueKey("EBBS_UQ_WRITEOFF");
                map.Index("EBBS_IX_WRITEOFF");
            });
            Property(x => x.CustomerName, map => map.NotNullable(false));
            Property(p => p.ChargeOffDate);
            Property(x => x.Product);
            Property(x => x.ProductName);
            Property(x => x.Branch);
            Property(x => x.IsSetteled);
            Property(x => x.TotalDue);
            Property(x => x.PrincipalDue);
            Property(p => p.InterestCharge);
            //Property(x => x.AmountRepaid);
            Property(x => x.CurrentDue);
            Property(x => x.Comments, map => map.NotNullable(false));
            Property(p => p.BounceCharge);
            Property(p => p.FeeCharge);
            Property(p => p.LateCharge);
            Property(x => x.IsReferred);
            Property(x => x.Pincode);
            Property(x => x.NoAllocResons);
            Property(x => x.CustStatus, map => map.NotNullable(false));
            Property(x => x.AllocStatus);
            #endregion

        }
    }
}