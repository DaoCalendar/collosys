#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class CWriteoffMap : EntityMap<CWriteoff>
    {
        public CWriteoffMap()
        {
            Table("C_WRITEOFF");
            Set(x => x.Allocs, colmap => { }, map => map.OneToMany(x => { }));
            ManyToOne(x => x.FileScheduler, map => map.NotNullable(true));

            Property(x => x.FileDate, map =>
            {
                map.UniqueKey("CCMS_UQ_WRITEOFF");
                map.Index("CCMS_IX_WRITEOFF");
            });

            Property(x => x.FileRowNo);
            Property(y => y.AccountNo, map =>
            {
                map.UniqueKey("CCMS_UQ_WRITEOFF");
                map.Index("CCMS_IX_WRITEOFF");
            });

            Property(x => x.CustomerName, map => map.NotNullable(false));
            Property(x => x.Cycle);
            Property(x => x.Location);
            Property(x => x.TotalDue);
            Property(x => x.LastPayDate);
            Property(x => x.IsReferred);
            Property(x => x.Block);
            Property(x => x.AltBlock);
            Property(x => x.Bucket1Due);
            Property(x => x.Bucket2Due);
            Property(x => x.Bucket3Due);
            Property(x => x.Bucket4Due);
            Property(x => x.Bucket5Due);
            Property(x => x.Bucket6Due);
            Property(x => x.Bucket7Due);

            Property(x => x.GlobalCustId);
            Property(x => x.Product);
            Property(x => x.ActivationDate);
            Property(x => x.ExpirtyDate);
            Property(x => x.CreditLimit);
            Property(x => x.Pincode);
            Property(x => x.NoAllocResons);

            Property(x=>x.AllocStatus);
            Property(x => x.CustStatus, map => map.NotNullable(false));
            ManyToOne(x => x.GPincode, map => { });

        }
    }
}



            //#region IDelq
            //Property(x => x.Pincode);
            //Property(x => x.DoAllocate, map => map.NotNullable(true));
            //Property(p => p.DelqAmount, map => map.NotNullable(true));
            //Property(p => p.DelqDate, map => map.NotNullable(true));
            //Property(p => p.DelqStatus, map =>
            //{
            //    map.NotNullable(true);
            //    map.Length(30);
            //});
            //#endregion
            //Property(x => x.InterestPct, map => map.NotNullable(true));

