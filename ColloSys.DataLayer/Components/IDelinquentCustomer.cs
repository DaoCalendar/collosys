using System;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Generic;

namespace ColloSys.DataLayer.Components
{
    public interface IDelinquentCustomer
    {
        string AccountNo { get; set; }
        ColloSysEnums.AllocStatus AllocStatus { get; set; }
        ScbEnums.Products Product { get; set; }
        bool IsReferred { get; set; }
        GPincode GPincode { get; set; } //cmi/add
        decimal TotalDue { get; set; } //cmi//add
        string CustomerName { get; set; } //cmi//add
        uint Pincode { get; set; }
        ColloSysEnums.NoAllocResons? NoAllocResons { get; set; }
        string CustStatus { get; set; }
        DateTime? AllocStartDate { get; set; }
        DateTime? AllocEndDate { get; set; }
    }
}