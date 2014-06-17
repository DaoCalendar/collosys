﻿#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class RLinerMap : EntityMap<RLiner>
    {
        public RLinerMap()
        {
            #region Relationship Mapping

            Set(x => x.Allocs, colmap => { }, map => map.OneToMany(x => { }));
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
            Property(x => x.Insurance);
            Property(x => x.PrincipalArrears);
            //ProductCode
            Property(x => x.BranchCode);
            Property(x => x.DSAID);
            Property(x => x.SCBSchemeCode);
            Property(x => x.ActualDisbursalDate);
            Property(x => x.FirstInstallment);
            Property(x => x.PrincipalProvision);
            Property(x => x.CurrentFSV);
            Property(x => x.FSVforProvision);
            Property(x => x.InterestProvision);
            Property(x => x.LateChargeProvision);
            Property(x => x.BounceChargesProvision);
            Property(x => x.FeeProvision);
            Property(x => x.InterestNonRevenue);
            Property(x => x.LateChargeNonRevenue);
            Property(x => x.BounceChargesNonRevenue);
            Property(x => x.FeeNonRevenue);
            Property(x => x.ImpairmentFlag);
            Property(x => x.ImpairmentMode);
            Property(x => x.DiscountProvision);
            Property(x => x.DiscountProvisionUnwind);
            Property(x => x.SecuritisationFlag);
            Property(x => x.CurrentValuationDate);
            Property(x => x.CurrentFSVDate);
            Property(x => x.RelatedLoanNumber1);
            Property(x => x.RelatedLoanNumber2);
            Property(x => x.RelatedLoanNumber3);
            Property(x => x.RelatedLoanNumber4);
            Property(x => x.RelatedLoanNumber5);
            Property(x => x.RelatedLoanNumber6);
            Property(x => x.RelatedLoanNumber7);
            Property(x => x.RelatedLoanNumber8);
            Property(x => x.RelatedLoanNumber9);
            Property(x => x.IASLoanBalance);
            Property(x => x.Totalofallprovisionfields);
            Property(x => x.Totalofallnon_revenues);
            Property(x => x.CustomerClass);
            Property(x => x.Employer);
            Property(x => x.PayrollFlag);
            Property(x => x.PDCFlag);
            Property(x => x.ECSFlag);
            Property(x => x.M2Reference);
            //FileDate 

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

