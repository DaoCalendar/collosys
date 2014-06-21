using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.ClientData
{
    public class ELinerSmeLapMap : EntityMap<ELinerSmeLap>
    {
        public ELinerSmeLapMap()
        {
            #region Relationship Mapping

            Set(x => x.Allocs, colmap => { }, map => map.OneToMany(x => { }));
            ManyToOne(x => x.FileScheduler, map => map.NotNullable(true));
            ManyToOne(x => x.GPincode, map => { });

            #endregion

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

            Property(x => x.ChargeOffDate);
            Property(x => x.PrincipalDue);
            Property(x => x.InterestCharge);
            Property(x => x.FeeCharge);
            Property(x => x.BounceCharge);
            Property(x => x.LateCharge);
            Property(x => x.TotalDue);
            Property(x => x.ProductName);
            Property(x => x.Product);
            Property(x => x.Branch);
            Property(x => x.CurrentDue);
            Property(x => x.IsSetteled);
            Property(x => x.CustStatus);
            Property(x => x.AllocStartDate);
            Property(x => x.AllocEndDate);
            Property(x => x.Comments);
            Property(x => x.FileDate);
            Property(x => x.FileRowNo);
            Property(x => x.IsReferred);
            Property(x => x.Pincode);
            Property(x => x.AllocStatus);
            Property(x => x.NoAllocResons);


            Property(x => x.CCY);
            Property(x => x.Branch_code);
            Property(x => x.AcOpenDate);
            Property(x => x.CRG);
            Property(x => x.ARM);
            Property(x => x.CurrentBalance);
            Property(x => x.AvailableBalance);
            Property(x => x.InterestLimit);
            Property(x => x.MLLAmount);
            Property(x => x.LimitLienAmt);
            Property(x => x.ExpiryDate);
            Property(x => x.MinimumRepayment);
            Property(x => x.TotalMinimumRepayment);
            Property(x => x.LimitProvnPdtCode);
            Property(x => x.ProvisionBalance);
            Property(x => x.InterestInSuspense);
            Property(x => x.FeesInSuspense);
            Property(x => x.DateOfReaging);
            Property(x => x.ReagingAmt);
            Property(x => x.DayPastDue);
            Property(x => x.LTV);
            Property(x => x.InterestRate);
            Property(x => x.DateOfExcess);
            Property(x => x.DaysInExcess);
            Property(x => x.ExcessAmt);
            Property(x => x.ImpairmentFlag);
            Property(x => x.ImpairmentDate);
            Property(x => x.PrincipleProvn);
            Property(x => x.InterestProvn);
            Property(x => x.FeeProvn);
            Property(x => x.InterestNonRevenue);
            Property(x => x.FeeNonRevenue);
            Property(x => x.StatementBalance);
            Property(x => x.TotalDues);
            Property(x => x.CycleDues);
            Property(x => x.CurrentRepaidAmt);
            Property(x => x.XDaysDue);
            Property(x => x.DPD_30_59);
            Property(x => x.DPD_60_89);
            Property(x => x.DPD_90_119);
            Property(x => x.DPD_120_149);
            Property(x => x.DPD_150_179);
            Property(x => x.DPD_180_209);
            Property(x => x.DPD_210_239);
            Property(x => x.DPD_240_269);
            Property(x => x.DPD_270_299);
            Property(x => x.DPD_300_329);
            Property(x => x.DPD_330_359);
            Property(x => x.DPD_360_389);
            Property(x => x.DPD_390_419);
            Property(x => x.DPD_420_449);
            Property(x => x.DPD_450_479);
            Property(x => x.DPD_480_509);
            Property(x => x.DPD_510_539);
            Property(x => x.DPD_540_569);
            Property(x => x.DPD_570_599);
            Property(x => x.DPD_600_629);
            Property(x => x.DPD_630_659);
            Property(x => x.DPD_660_689);
            Property(x => x.DPD_690_719);
            Property(x => x.DPD_720);
            Property(x => x.OverlimitFlag);
            Property(x => x.OVLDays);
            Property(x => x.OVLAmount);
            Property(x => x.LimitSuspensionFlag);
            Property(x => x.RelatedLoanNo);
            Property(x => x.FsvForProvn);
            Property(x => x.FsvForProvisionDate);
            Property(x => x.CurrentFSV);
            Property(x => x.CurrentFSVDate);
            Property(x => x.ProvnLevel);
            Property(x => x.TempInterestProvn);
            Property(x => x.TempFeeProvn);
            Property(x => x.TotalDiscountedFSV);
            Property(x => x.DiscountProvnStartDate);
            Property(x => x.DiscountPeriod);
            Property(x => x.RemainingDiscountPeriod);
            Property(x => x.TotalDiscountProvision);
            Property(x => x.TotalDiscountProvisionRelease);
            Property(x => x.RemainingDiscountProvn);
            Property(x => x.TotalUnwindingAmt);
            Property(x => x.LastUnwindingDate);
            Property(x => x.LastUnwindingAmt);

        }
    }
}
