#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.SharedDomain
{
    public class CustomerInfo : Entity, IDeliquent
    {
        public virtual Iesi.Collections.Generic.ISet<Allocations> Allocs { get; set; }

        #region Properties

        public virtual ColloSysEnums.DelqFlag Flag { get; set; }

        public virtual string AccountNo { get; set; }

        public virtual string GlobalCustId { get; set; }

        public virtual string CustomerName { get; set; }

        public virtual uint Pincode { get; set; }

        public virtual ScbEnums.Products Product { get; set; }

        public virtual string CustStatus { get; set; }

        public virtual DateTime? AllocStartDate { get; set; }

        public virtual DateTime? AllocEndDate { get; set; }

        public virtual DateTime? ChargeofDate { get; set; }

        public virtual bool IsInRecovery { get; set; }

        public virtual ColloSysEnums.AllocStatus AllocStatus { get; set; }

        public virtual bool IsReferred { get; set; }

        public virtual bool IsXHoldAccount { get; set; }

        public virtual decimal TotalDue { get; set; }

        public virtual ColloSysEnums.NoAllocResons? NoAllocResons { get; set; }

        public virtual uint Cycle { get; set; }

        public virtual uint Bucket { get; set; }

        public virtual GPincode GPincode { get; set; }

        #endregion

        #region demo ICICI
        //TODO: remove post demo
        public virtual string LanNo { get; set; }
        public virtual string Zone { get; set; }
        public virtual string Region { get; set; }
        public virtual string Location { get; set; }
        public virtual string CustName { get; set; }
        public virtual ulong SanctionAmt { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime SanctionDate { get; set; }
        public virtual DateTime AgreementDate { get; set; }
        public virtual string CustCat { get; set; }
        public virtual decimal IRR { get; set; }
        public virtual ulong Tenure { get; set; }
        public virtual string RepaymentMode { get; set; }
        public virtual ulong? AssetCode { get; set; }
        public virtual ColloSysEnums.DelqFlag? AssetType { get; set; }
        public virtual string Scheme { get; set; }
        public virtual string DisbMemoNo { get; set; }
        public virtual DateTime DisbMemoDate { get; set; }
        public virtual ulong? ProcessingFees { get; set; }
        public virtual ulong NetDisb { get; set; }
        public virtual ulong DisbAmt { get; set; }
        public virtual string DisbMode { get; set; }
        public virtual string DisbStatus { get; set; }
        public virtual ulong? EmpIdCredit { get; set; }
        public virtual string EmpIdOps { get; set; }
        public virtual string LoanSource { get; set; }
        public virtual long? DMACode { get; set; }
        public virtual ColloSysEnums.CityCategory CityCat { get; set; }
        public virtual string LoanType { get; set; }
        public virtual DateTime? MemoApprovalDate { get; set; }
        #endregion
    }
}
