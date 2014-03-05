﻿#region references

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
    public class Info : Entity, IDeliquent
    {
        public virtual Iesi.Collections.Generic.ISet<Alloc> Allocs { get; set; }

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
       
    }
}

//#region Relationship - None
//public override void MakeEmpty(bool forceEmpty = false)
//{
//}
//#endregion
//    public virtual bool DoAllocate { get; set; }
//    public virtual DateTime? AllocStartDate { get; set; }
//    public virtual DateTime? AllocEndDate { get; set; }
//    public virtual ulong AccountNo { get; set; }
//    public virtual Allocation.AllocPolicy AllocPolicy { get; set; }
//    public virtual Allocation.AllocSubpolicy AllocSubpolicy { get; set; }

