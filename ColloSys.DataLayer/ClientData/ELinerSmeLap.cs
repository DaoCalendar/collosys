using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.SharedDomain;


namespace ColloSys.DataLayer.ClientData
{
    public class ELinerSmeLap : UploadableEntity, IDelinquentCustomer, IUniqueKey
    {
        #region Properties
        public virtual string AccountNo { get; set; }
        public virtual DateTime ChargeOffDate { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual decimal PrincipalDue { get; set; }
        public virtual decimal InterestCharge { get; set; }
        public virtual decimal FeeCharge { get; set; }
        public virtual decimal BounceCharge { get; set; }
        public virtual decimal LateCharge { get; set; }
        public virtual decimal TotalDue { get; set; }
        public virtual string ProductName { get; set; }
        public virtual ScbEnums.Products Product { get; set; }
        public virtual string Branch { get; set; }
        public virtual decimal CurrentDue { get; set; }
        public virtual bool IsSetteled { get; set; }
        public virtual string CustStatus { get; set; }
        public virtual DateTime? AllocStartDate { get; set; }
        public virtual DateTime? AllocEndDate { get; set; }
        public virtual string Comments { get; set; }
        public override DateTime FileDate { get; set; }
        public override ulong FileRowNo { get; set; }
        public virtual bool IsReferred { get; set; }
        public virtual uint Pincode { get; set; }
        public virtual ColloSysEnums.AllocStatus AllocStatus { get; set; }
        public virtual ColloSysEnums.NoAllocResons? NoAllocResons { get; set; }
        public virtual GPincode GPincode { get; set; }
        public override FileScheduler FileScheduler { get; set; }

        public virtual string CCY { get; set; }
        public virtual string Branch_code { get; set; }
        public virtual string AcOpenDate { get; set; }
        public virtual string CRG { get; set; }
        public virtual string ARM { get; set; }
        public virtual string CurrentBalance { get; set; }
        public virtual string AvailableBalance { get; set; }
        public virtual string InterestLimit { get; set; }
        public virtual string MLLAmount { get; set; }
        public virtual string LimitLienAmt { get; set; }
        public virtual string ExpiryDate { get; set; }
        public virtual string MinimumRepayment { get; set; }
        public virtual string TotalMinimumRepayment { get; set; }
        public virtual string LimitProvnPdtCode { get; set; }
        public virtual string ProvisionBalance { get; set; }
        public virtual string InterestInSuspense { get; set; }
        public virtual string FeesInSuspense { get; set; }
        public virtual string DateOfReaging { get; set; }
        public virtual string ReagingAmt { get; set; }
        public virtual string DayPastDue { get; set; }
        public virtual string LTV { get; set; }
        public virtual string InterestRate { get; set; }
        public virtual string DateOfExcess { get; set; }
        public virtual string DaysInExcess { get; set; }
        public virtual string ExcessAmt { get; set; }
        public virtual string ImpairmentFlag { get; set; }
        public virtual string ImpairmentDate { get; set; }
        public virtual string PrincipleProvn { get; set; }
        public virtual string InterestProvn { get; set; }
        public virtual string FeeProvn { get; set; }
        public virtual string InterestNonRevenue { get; set; }
        public virtual string FeeNonRevenue { get; set; }
        public virtual string StatementBalance { get; set; }
        public virtual string TotalDues { get; set; }
        public virtual string CycleDues { get; set; }
        public virtual string CurrentRepaidAmt { get; set; }
        public virtual string XDaysDue { get; set; }
        public virtual string DPD_30_59 { get; set; }
        public virtual string DPD_60_89 { get; set; }
        public virtual string DPD_90_119 { get; set; }
        public virtual string DPD_120_149 { get; set; }
        public virtual string DPD_150_179 { get; set; }
        public virtual string DPD_180_209 { get; set; }
        public virtual string DPD_210_239 { get; set; }
        public virtual string DPD_240_269 { get; set; }
        public virtual string DPD_270_299 { get; set; }
        public virtual string DPD_300_329 { get; set; }
        public virtual string DPD_330_359 { get; set; }
        public virtual string DPD_360_389 { get; set; }
        public virtual string DPD_390_419 { get; set; }
        public virtual string DPD_420_449 { get; set; }
        public virtual string DPD_450_479 { get; set; }
        public virtual string DPD_480_509 { get; set; }
        public virtual string DPD_510_539 { get; set; }
        public virtual string DPD_540_569 { get; set; }
        public virtual string DPD_570_599 { get; set; }
        public virtual string DPD_600_629 { get; set; }
        public virtual string DPD_630_659 { get; set; }
        public virtual string DPD_660_689 { get; set; }
        public virtual string DPD_690_719 { get; set; }
        public virtual string DPD_720 { get; set; }
        public virtual string OverlimitFlag { get; set; }
        public virtual string OVLDays { get; set; }
        public virtual string OVLAmount { get; set; }
        public virtual string LimitSuspensionFlag { get; set; }
        public virtual string RelatedLoanNo { get; set; }
        public virtual string FsvForProvn { get; set; }
        public virtual string FsvForProvisionDate { get; set; }
        public virtual string CurrentFSV { get; set; }
        public virtual string CurrentFSVDate { get; set; }
        public virtual string ProvnLevel { get; set; }
        public virtual string TempInterestProvn { get; set; }
        public virtual string TempFeeProvn { get; set; }
        public virtual string TotalDiscountedFSV { get; set; }
        public virtual string DiscountProvnStartDate { get; set; }
        public virtual string DiscountPeriod { get; set; }
        public virtual string RemainingDiscountPeriod { get; set; }
        public virtual string TotalDiscountProvision { get; set; }
        public virtual string TotalDiscountProvisionRelease { get; set; }
        public virtual string RemainingDiscountProvn { get; set; }
        public virtual string TotalUnwindingAmt { get; set; }
        public virtual string LastUnwindingDate { get; set; }
        public virtual string LastUnwindingAmt { get; set; }

        #endregion

        #region Relationship

        public override IList<string> GetExcludeInExcelProperties()
        {
            var memberHelper = new MemberHelper<EWriteoff>();
            return new List<string> {
                memberHelper.GetName(x => x.Id),
                memberHelper.GetName(x => x.Version),
                memberHelper.GetName(x => x.CreatedBy),
                memberHelper.GetName(x => x.CreatedOn),
                memberHelper.GetName(x => x.CreateAction),
                memberHelper.GetName(x => x.IsReferred),
                memberHelper.GetName(x => x.Pincode),
                memberHelper.GetName(x => x.AllocStatus),
                memberHelper.GetName(x => x.NoAllocResons),
                memberHelper.GetName(x => x.FileScheduler),
                memberHelper.GetName(x => x.GPincode),
                memberHelper.GetName(x => x.Allocs)
            };
        }

        public override IList<string> GetWriteInExcelProperties(ColloSysEnums.FileAliasName? aliasName = null)
        {
            var memberHelper = new MemberHelper<EWriteoff>();
            return new List<string>
                {
                    memberHelper.GetName(x => x.AccountNo),
                    memberHelper.GetName(x => x.ChargeOffDate),
                    memberHelper.GetName(x => x.CustomerName),
                    memberHelper.GetName(x => x.PrincipalDue),
                    memberHelper.GetName(x => x.InterestCharge),
                    memberHelper.GetName(x => x.FeeCharge ),
                    memberHelper.GetName(x => x.BounceCharge ),
                    memberHelper.GetName(x => x.LateCharge ),
                    memberHelper.GetName(x => x.TotalDue ),
                    memberHelper.GetName(x => x.ProductName ),
                    memberHelper.GetName(x => x.Product ),
                    memberHelper.GetName(x => x.Branch ),
                    memberHelper.GetName(x => x.FinalAmountDue ),
                    memberHelper.GetName(x => x.IsSetteled ),
                    memberHelper.GetName(x => x.CustStatus ),
                    memberHelper.GetName(x => x.Remarks ),
                    memberHelper.GetName(x => x.FileDate ),
                    memberHelper.GetName(x => x.FileRowNo ),


                };
        }

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(EAllocs) || forceEmpty) EAllocs = null;
        //}

        public virtual Iesi.Collections.Generic.ISet<Allocations> Allocs { get; set; }

        #endregion
    }
}
