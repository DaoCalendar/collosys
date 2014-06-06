using System;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Billing
{
    public interface ICustBillViewModel
    {
        string AccountNo { get; set; }

        string GlobalCustId { get; set; }

        ColloSysEnums.DelqFlag Flag { get; set; }

        ScbEnums.Products Product { get; set; }

        bool IsInRecovery { get; set; }

        DateTime? ChargeofDate { get; set; }

        uint Cycle { get; set; }

        uint Bucket { get; set; }

        uint MobWriteoff { get; set; }

        uint Vintage { get; set; }

        ColloSysEnums.CityCategory CityCategory { get; set; }

        string City { get; set; }

        bool IsXHoldAccount { get; set; }

        DateTime AllocationStartDate { get; set; }

        DateTime? AllocationEndDate { get; set; }

        decimal TotalDueOnAllocation { get; set; }

        decimal TotalAmountRecovered { get; set; }

        decimal ResolutionPercentage { get; set; }
    }
}
