using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Stakeholder
{
    public class Stakeholders : Entity
    {
        public virtual IList<BillAdhoc> BillAdhocs { get; set; }
        public virtual IList<BillSummary> BillAmounts { get; set; }
        public virtual IList<BillDetail> BillDetails { get; set; }
        public virtual IList<Allocations> Allocs { get; set; }
        public virtual IList<StkhHoldingPolicy> ActivateHoldingPolicies { get; set; }
        public virtual IList<StkhPayment> StkhPayments { get; set; }
        public virtual IList<StkhRegistration> StkhRegistrations { get; set; }
        public virtual IList<StkhWorking> StkhWorkings { get; set; }
        public virtual IList<StkhAddress> StkhAddress { get; set; }
        public virtual IList<AllocSubpolicy> AllocSubpolicies { get; set; }
        public virtual IList<Allocations> ReportsAllocs { get; set; }

        public virtual string ExternalId { get; set; }
        public virtual string Name { get; set; }
        public virtual string MobileNo { get; set; }
        public virtual string EmailId { get; set; }
        public virtual string Password { get; set; }
        public virtual Guid ReportingManager { get; set; }
        public virtual DateTime JoiningDate { get; set; }
        public virtual DateTime? LeavingDate { get; set; }
        public virtual StkhHierarchy Hierarchy { get; set; }

        public virtual ColloSysEnums.ApproveStatus ApprovalStatus { get; set; }
    }
}
