using ColloSys.DataLayer.Generic;

#region References

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

#endregion

// ReSharper disable CheckNamespace
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ColloSys.DataLayer.Domain
{
    public class RLiner : UploadableEntity, ISoftDelq, IUniqueKey
    {
        #region Properties

        public virtual ColloSysEnums.DelqFlag Flag { get; set; }
        public virtual decimal PrincipalDue { get; set; }
        public virtual string DelqHistoryString { get; set; }

        public virtual string AccountNo { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual decimal LoanTotalDue { get; set; }
        public virtual decimal InterestCharge { get; set; }
        public virtual decimal LateCharge { get; set; }
        public virtual decimal BounceCharge { get; set; }
        public virtual decimal FeeCharge { get; set; }
        public virtual decimal TotalDue { get; set; }
        public virtual decimal RevisedDue { get; set; }
        public virtual decimal Emi { get; set; }
        public virtual decimal EmiDue { get; set; }
        public virtual string Branch { get; set; }
        public virtual uint Tenure { get; set; }
        public virtual DateTime? FirstInstDate { get; set; }
        public virtual DateTime? FinalInstDate { get; set; }
        public virtual uint ProductCode { get; set; }
        public virtual string ProductName { get; set; }
        public virtual ScbEnums.Products Product { get; set; }
        public virtual string AgeCode { get; set; }
        public virtual uint Bucket { get; set; }
        public virtual decimal LoanPrinDue { get; set; }
        public virtual uint Cycle { get; set; }
        public virtual bool IsImpaired { get; set; }
        public virtual string CustStatus { get; set; }
        public virtual DateTime? AllocStartDate { get; set; }
        public virtual DateTime? AllocEndDate { get; set; }
        public override DateTime FileDate { get; set; }
        public override ulong FileRowNo { get; set; }

        public virtual bool IsReferred { get; set; }
        public virtual uint Pincode { get; set; }
        public virtual ColloSysEnums.AllocStatus AllocStatus { get; set; }
        public virtual ColloSysEnums.NoAllocResons? NoAllocResons { get; set; }
        public override FileScheduler FileScheduler { get; set; }
        public virtual GPincode GPincode { get; set; }

        public virtual string Insurance { get; set; }
        public virtual string PrincipalArrears { get; set; }
        //public virtual string ProductCode { get; set; } already exist
        public virtual string BranchCode { get; set; }
        public virtual string DSAID { get; set; }
        public virtual string SCBSchemeCode { get; set; }
        public virtual string ActualDisbursalDate { get; set; }
        public virtual string FirstInstallment { get; set; }
        public virtual string PrincipalProvision { get; set; }
        public virtual string CurrentFSV { get; set; }
        public virtual string FSVforProvision { get; set; }
        public virtual string InterestProvision { get; set; }
        public virtual string LateChargeProvision { get; set; }
        public virtual string BounceChargesProvision { get; set; }
        public virtual string FeeProvision { get; set; }
        public virtual string InterestNonRevenue { get; set; }
        public virtual string LateChargeNonRevenue { get; set; }
        public virtual string BounceChargesNonRevenue { get; set; }
        public virtual string FeeNonRevenue { get; set; }
        public virtual string ImpairmentFlag { get; set; }
        public virtual string ImpairmentMode { get; set; }
        public virtual string DiscountProvision { get; set; }
        public virtual string DiscountProvisionUnwind { get; set; }
        public virtual string SecuritisationFlag { get; set; }
        public virtual string CurrentValuationDateDDMMYYYY { get; set; }
        public virtual string CurrentFSVDateDDMMYYYY { get; set; }
        public virtual string RelatedLoanNumber1 { get; set; }
        public virtual string RelatedLoanNumber2 { get; set; }
        public virtual string RelatedLoanNumber3 { get; set; }
        public virtual string RelatedLoanNumber4 { get; set; }
        public virtual string RelatedLoanNumber5 { get; set; }
        public virtual string RelatedLoanNumber6 { get; set; }
        public virtual string RelatedLoanNumber7 { get; set; }
        public virtual string RelatedLoanNumber8 { get; set; }
        public virtual string RelatedLoanNumber9 { get; set; }
        public virtual string IASLoanBalance { get; set; }
        public virtual string Totalofallprovisionfields { get; set; }
        public virtual string Totalofallnonrevenues { get; set; }
        public virtual string CustomerClass { get; set; }
        public virtual string Employer { get; set; }
        public virtual string PayrollFlag { get; set; } //present in 2 files but not in one (ExcelSheet)
        public virtual string PDCFlag { get; set; }
        public virtual string ECSFlag { get; set; }
        public virtual string M2Reference { get; set; }
        //FileDate already exist 

        #endregion

        #region Relationship

        public override IList<string> GetExcludeInExcelProperties()
        {
            var memberHelper = new MemberHelper<RLiner>();
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
            var memberHelper = new MemberHelper<RLiner>();
            return new List<string>
                {
                    memberHelper.GetName(x => x.AccountNo ),
                    memberHelper.GetName(x => x.CustomerName ),
                    memberHelper.GetName(x => x.LoanTotalDue ),
                    memberHelper.GetName(x => x.InterestCharge ),
                    memberHelper.GetName(x => x.LateCharge ),
                    memberHelper.GetName(x => x.BounceCharge ),
                    memberHelper.GetName(x => x.FeeCharge ),
                    memberHelper.GetName(x => x.TotalDue ),
                    memberHelper.GetName(x => x.RevisedDue ),
                    memberHelper.GetName(x => x.Emi ),
                    memberHelper.GetName(x => x.EmiDue ),
                    memberHelper.GetName(x => x.Branch ),
                    memberHelper.GetName(x => x.Tenure ),
                    memberHelper.GetName(x => x.FirstInstDate ),
                    memberHelper.GetName(x => x.FinalInstDate ),
                    memberHelper.GetName(x => x.ProductName ),
                    memberHelper.GetName(x => x.Product ),
                    memberHelper.GetName(x => x.AgeCode ),
                    memberHelper.GetName(x => x.Bucket),
                    memberHelper.GetName(x => x.LoanPrinDue ),
                    memberHelper.GetName(x => x.Cycle ),
                    memberHelper.GetName(x => x.IsImpaired),
                    memberHelper.GetName(x => x.CustStatus),
                    memberHelper.GetName(x => x.FileDate ),
                    memberHelper.GetName(x => x.FileRowNo ),

     

                };
        }

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(RAllocs) || forceEmpty) RAllocs = null;
        //}
        public virtual Iesi.Collections.Generic.ISet<Allocations> Allocs { get; set; }

        #endregion

        #region NotMapped Fields



        #endregion
    }
}

