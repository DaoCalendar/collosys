using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

namespace ColloSys.DataLayer.ClientData
{

    public class InfoMap<T> : EntityMap<T> where T : Info
    {
        public InfoMap(ScbEnums.ScbSystems tableName)
        {
            var firstchar = tableName.ToString()[0];
            Table(firstchar + "_INFO");
            Property(x => x.AccountNo, map => map.UniqueKey("UK_" + firstchar + "INFO_ACNO"));
            Property(x => x.GlobalCustId, map => map.NotNullable(false));
            Property(x => x.CustomerName, map => map.NotNullable(false));
            Property(x => x.Pincode);
            Property(x => x.Product);
            Property(x => x.IsInRecovery);
            Property(x => x.CustStatus, map => map.NotNullable(false));
            Property(x => x.AllocStatus);
            Property(x => x.Flag);
            Property(x => x.Cycle);
            Property(x => x.Bucket);
            Property(x => x.TotalDue);
            Property(x => x.IsReferred);
            Property(x => x.IsXHoldAccount);
            Property(x => x.NoAllocResons);
            Property(x => x.AllocEndDate);
            Property(x => x.AllocStartDate);
            Property(x => x.ChargeofDate, map => map.NotNullable(false));
           ManyToOne(x => x.GPincode, map => { });
           Set(x => x.EAllocs, colmap => { }, map => map.OneToMany(x => { }));
        }
    }

    public class CInfoMap : InfoMap<CInfo>
    {
        public CInfoMap() : base(ScbEnums.ScbSystems.CCMS)
        {
            Property(x => x.FileDate);

            Property(x => x.FileRowNo);

            Property(x => x.CreditLimit);
            Property(x => x.Cycle);
            Property(x => x.Location);
            Property(x => x.IsReferred);
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
            Property(x => x.NoAllocResons);

            
            Property(x => x.DelqHistoryString);
            Property(x => x.PeakBucket);
            Property(x => x.AccountStatus);

            Property(x => x.AllocStatus);
            Set(x => x.CAllocs, colmap => { }, map => map.OneToMany(x => { }));
        }
    }

}