using ColloSys.DataLayer.Generic;

#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

#endregion

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable CheckNamespace
namespace ColloSys.DataLayer.Domain
{
    public class RWriteoff : UploadableEntity, IDelinquentCustomer, IUniqueKey
    {
        #region Properties
        public virtual string AccountNo { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual DateTime? DisbursementDate { get; set; }
        public virtual DateTime? FinalInstDate { get; set; }
        public virtual uint Cycle { get; set; }
        public virtual string Branch { get; set; }
        public virtual DateTime? ChargeOffDate { get; set; }
        public virtual string ProductName { get; set; }
        public virtual ScbEnums.Products Product { get; set; }
        public virtual decimal BounceCharge { get; set; }
        public virtual decimal LateCharge { get; set; }
        public virtual decimal PrincipalDue { get; set; }
        public virtual decimal InterestCharge { get; set; }
        public virtual decimal FeeCharge { get; set; }
        public virtual decimal TotalDue { get; set; }
        public virtual decimal Recovery { get; set; }
        public virtual decimal CurrentDue { get; set; }
        public virtual bool IsSetteled { get; set; }
        public virtual string Comment { get; set; }
        public virtual string CustStatus { get; set; }
        public virtual DateTime? AllocStartDate { get; set; }
        public virtual DateTime? AllocEndDate { get; set; }
        public override DateTime FileDate { get; set; }
        public override ulong FileRowNo { get; set; }

        public virtual bool IsReferred { get; set; }
        public virtual uint Pincode { get; set; }
        public virtual ColloSysEnums.AllocStatus AllocStatus { get; set; }
        public virtual ColloSysEnums.NoAllocResons? NoAllocResons { get; set; }
        public virtual GPincode GPincode { get; set; }
        public override FileScheduler FileScheduler { get; set; }
        #endregion

        #region Relationship

        public override IList<string> GetExcludeInExcelProperties()
        {
            var memberHelper = new MemberHelper<RWriteoff>();
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

        public virtual Iesi.Collections.Generic.ISet<Allocations> Allocs { get; set; }

        #endregion
    }
}

// ReSharper restore CheckNamespace
// ReSharper restore ClassWithVirtualMembersNeverInherited.Global
// ReSharper restore DoNotCallOverridableMethodsInConstructor
// ReSharper restore UnusedAutoPropertyAccessor.Global
// ReSharper restore MemberCanBeProtected.Global

//public virtual decimal LoanAmount { get; set; }

//public virtual UInt32 Tenure { get; set; }

//public virtual decimal? Emi { get; set; }

//public virtual string DelqStatus { get; set; }

//public virtual DateTime DelqDate { get; set; }

//public virtual decimal DelqAmount { get; set; }

//public virtual uint? Pincode { get; set; }

//public virtual bool DoAllocate { get; set; }

//public virtual decimal InterestPct { get; set; }

