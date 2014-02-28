using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;

namespace ColloSys.DataLayer.Billing
{
    public class CustBillViewModel
    {
        public string AccountNo { get; set; }

        public string GlobalCustId { get; set; }

        public ColloSysEnums.DelqFlag Flag { get; set; }

        public ScbEnums.Products Product { get; set; }

        public bool IsInRecovery { get; set; }

        public DateTime? ChargeofDate { get; set; }

        public uint Cycle { get; set; }

        public uint Bucket { get; set; }

        public uint MobWriteoff { get; set; }

        public uint Vintage { get; set; }

        public ColloSysEnums.CityCategory CityCategory { get; set; }

        public bool IsXHoldAccount { get; set; }

        public DateTime AllocationStartDate { get; set; }

        public DateTime? AllocationEndDate { get; set; }

        public decimal TotalDueOnAllocation { get; set; }

        public decimal TotalAmountRecovered { get; set; }

        public decimal ResolutionPercentage { get; set; }

        public GPincode GPincode { get; set; }

        public Stakeholders Stakeholders { get; set; }

        //public StkhPerformance StkhPerformance { get; set; }
    }

    public class StkhBillViewModel
    {
        public Stakeholders Stakeholders { get; set; }

        public decimal TotalAmountRecovered { get; set; }

        public decimal TotalAmountAllocated { get; set; }

        public decimal TotalResolutionPer
        {
            get { return (TotalAmountRecovered * (100)) / TotalAmountAllocated; }
        }


        public decimal Bucket1ResolutionPer { get; set; }

        public decimal Bucket2ResolutionPer { get; set; }

        public decimal Bucket3ResolutionPer { get; set; }

        public decimal Bucket4ResolutionPer { get; set; }

        public decimal Bucket5ResolutionPer { get; set; }

        public decimal Bucket6ResolutionPer { get; set; }
    }

    public class CustStkhBillViewModel
    {
        public CustBillViewModel CustBillViewModel { get; set; }

        public GPincode GPincode { get; set; }

        public StkhBillViewModel StkhBillViewModel { get; set; }
    }
}
