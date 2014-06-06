using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Billing
{
    class CustBillViewModelMap : EntityMap<CustBillViewModel>
    {
        public CustBillViewModelMap()
        {
            Property(x => x.AccountNo);
            Property(x => x.GlobalCustId);
            Property(x => x.Flag);
            Property(x => x.Product);
            Property(x => x.IsInRecovery);
            Property(x => x.ChargeofDate);
            Property(x => x.Cycle);
            Property(x => x.Bucket);
            Property(x => x.MobWriteoff);
            Property(x => x.MobWriteoff);
            Property(x => x.Vintage);
            Property(x => x.CityCategory);
            Property(x => x.City);
            Property(x => x.IsXHoldAccount);
            Property(x => x.AllocationStartDate);
            Property(x => x.AllocationEndDate);
            Property(x => x.TotalDueOnAllocation);
            Property(x => x.TotalAmountRecovered);
            Property(x => x.ResolutionPercentage);
            Property(x => x.ConditionSatisfy, map => map.Length(4001));

            ManyToOne(x => x.BillDetail);
            ManyToOne(x => x.GPincode);
            ManyToOne(x => x.Stakeholders);
        }
    }
}
