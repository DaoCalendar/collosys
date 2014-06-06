using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;

namespace ColloSys.DataLayer.Billing
{
    public class BillStatus : Entity
    {
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual UInt32 BillMonth { get; set; }
        public virtual UInt32 OriginMonth { get; set; }
        public virtual UInt32 BillCycle { get; set; }
        public virtual ColloSysEnums.BillingStatus Status { get; set; }
        public virtual ScbEnums.Products Products { get; set; }
        public virtual string ExternalId { get; set; }
    }
}