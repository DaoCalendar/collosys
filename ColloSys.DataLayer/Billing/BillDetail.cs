#region References
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class BillDetail : Entity
    {
        public virtual IList<CustBillViewModel> CustBillViewModels { get; set; }
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual BillingPolicy BillingPolicy { get; set; }
        public virtual BillingSubpolicy BillingSubpolicy { get; set; }
        public virtual BillAdhoc BillAdhoc { get; set; }

        public virtual uint BillMonth { get; set; }

        public virtual uint BillCycle { get; set; }

        public virtual decimal Amount { get; set; }
      
        public virtual ScbEnums.Products Products { get; set; }

        public virtual ColloSysEnums.PaymentSource PaymentSource { get; set; }

        public virtual string TraceLog { get; set; }
    }
}