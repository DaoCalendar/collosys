using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;

namespace ColloSys.DataLayer.Billing
{
    public class BillDetail : Entity
    {
        public virtual IList<CustBillViewModel> CustBillViewModels { get; set; }
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual BillingPolicy BillingPolicy { get; set; }
        public virtual BillingSubpolicy BillingSubpolicy { get; set; }
        public virtual BillAdhoc BillAdhoc { get; set; }

        public virtual uint BillMonth { get; set; }
        public virtual uint OriginMonth { get; set; }
        public virtual uint BillCycle { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual decimal BaseAmount { get; set; }
      
        public virtual ScbEnums.Products Products { get; set; }
        public virtual ColloSysEnums.PaymentSource PaymentSource { get; set; }
        public virtual string TraceLog { get; set; }
        public virtual ColloSysEnums.PolicyType PolicyType { get; set; }
    }
}