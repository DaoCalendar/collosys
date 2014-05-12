using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Billing
{
    public class CustBillViewModel : Entity, ICustBillViewModel
    {

        #region SCB
        public virtual string AccountNo { get; set; }

        public virtual string GlobalCustId { get; set; }

        public virtual ColloSysEnums.DelqFlag Flag { get; set; }

        public virtual ScbEnums.Products Product { get; set; }

        public virtual bool IsInRecovery { get; set; }

        public virtual DateTime? ChargeofDate { get; set; }

        public virtual uint Cycle { get; set; }

        public virtual uint Bucket { get; set; }

        public virtual uint MobWriteoff { get; set; }

        public virtual uint Vintage { get; set; }

        public virtual ColloSysEnums.CityCategory CityCategory { get; set; }

        public virtual string City { get; set; }

        public virtual bool IsXHoldAccount { get; set; }

        public virtual DateTime AllocationStartDate { get; set; }

        public virtual DateTime? AllocationEndDate { get; set; }

        public virtual decimal TotalDueOnAllocation { get; set; }

        public virtual decimal TotalAmountRecovered { get; set; }

        public virtual decimal ResolutionPercentage { get; set; }

        public virtual GPincode GPincode { get; set; }

        public virtual Stakeholders Stakeholders { get; set; }

        public virtual string ConditionSatisfy { get; set; }

        public virtual BillDetail BillDetail { get; set; }

        #endregion

        //#region demo ICICI
        ////TODO: remove post demo
        
        //public virtual string Zone { get; set; }
        //public virtual string Region { get; set; }
        //public virtual string Location { get; set; }
        //public virtual string CustName { get; set; }
        
        //public virtual DateTime StartDate { get; set; }
        //public virtual DateTime SanctionDate { get; set; }
        //public virtual DateTime AgreementDate { get; set; }
        //public virtual string CustCat { get; set; }
        //public virtual decimal IRR { get; set; }
        //public virtual ulong Tenure { get; set; }
        //public virtual string RepaymentMode { get; set; }
        //public virtual ulong? AssetCode { get; set; }
        //public virtual ColloSysEnums.DelqFlag? AssetType { get; set; }
        //public virtual string Scheme { get; set; }
        //public virtual string DisbMemoNo { get; set; }
        //public virtual DateTime DisbMemoDate { get; set; }
        
        //public virtual ulong NetDisb { get; set; }
        
        //public virtual string DisbMode { get; set; }
        //public virtual string DisbStatus { get; set; }
        //public virtual ulong? EmpIdCredit { get; set; }
        //public virtual string EmpIdOps { get; set; }
        //public virtual string LoanSource { get; set; }
        //public virtual long? DMACode { get; set; }
        //public virtual ColloSysEnums.CityCategory CityCat { get; set; }
        //public virtual string LoanType { get; set; }
        //public virtual DateTime? MemoApprovalDate { get; set; }

        //public virtual string SubProduct { get; set; }

        //public virtual ulong TotalPF { get; set; }
        

       
        //#endregion

        #region Demo DHFL
        public virtual string LanNo { get; set; }
        public virtual ulong SanctionAmt { get; set; }
        public virtual DateTime Month { get; set; }
        public virtual ulong DisbAmt { get; set; }
        public virtual ulong? ProcessingFees { get; set; }
        public virtual ulong TotalDisbAmt { get; set; }
        public virtual ulong TotalProcFee { get; set; }
        public virtual ulong Payout { get; set; }
        public virtual ulong TotalPayout { get; set; }
        public virtual ulong DeductCap { get; set; }
        public virtual ulong DeductPf { get; set; }
        public virtual ulong FinalPayout { get; set; }

        #region input file columns

        public virtual string BranchName { get; set; }
        public virtual string Branchcat { get; set; }
        public virtual uint ApplNo { get; set; }
        public virtual uint Loancode { get; set; }
        public virtual uint SalesRefNo { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime SanctionDt { get; set; }
        public virtual ulong SanAmt { get; set; }
        public virtual DateTime DisbursementDt { get; set; }
        public virtual ulong DisbursementAmt { get; set; }
        public virtual uint FeeDue { get; set; }
        public virtual uint FeeWaived { get; set; }
        public virtual uint FeeReceived { get; set; }
        public virtual string MemberName { get; set; }
        public virtual string DesigName { get; set; }
        public virtual string Orignateby { get; set; }
        public virtual string Orignateby2 { get; set; }
        public virtual string Orignateby3 { get; set; }
        public virtual string Orignateby4 { get; set; }
        public virtual string Orignateby5 { get; set; }
        public virtual string OCCUPCATEGORY { get; set; }
        public virtual string REFERRALTYPE { get; set; }
        public virtual string REFERRALNAME { get; set; }
        public virtual string REFERRALCODE { get; set; }
        public virtual string SOURCENAME { get; set; }
        public virtual string SchemeGroupName { get; set; }
        public virtual string M_SCHNAME { get; set; }
        public virtual string M_SCHNAMEPremium { get; set; }

        #endregion

        #endregion

        //LoanNo- Account No
        //    SanctionAmt	
        //    Month	
        //        DisbAmt	
        //        ProcFee- ProcessingFees	
        //            TotalDisbAmt	
        //            TotalProcFee	
        //                Payout	
        //                TotalPayout	
        //                    DeductCap	
        //                    DeductPF	
        //                        FinalPayout

    }
}
