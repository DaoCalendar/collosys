using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Stakeholder;

namespace ColloSys.DataLayer.Billing
{
    public class BillAdhoc : Entity
    {
        public virtual IList<BillDetail> BillDetails { get; set; }
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual  ScbEnums.Products Products{ get; set; }
        public virtual decimal TotalAmount { get; set; }
        public virtual decimal RemainingAmount { get; set; }
        public virtual bool IsRecurring { get; set; }
        public virtual bool IsCredit { get; set; }
        public virtual string IsPretax { get; set; }
        public virtual string ReasonCode { get; set; }
        public virtual uint StartMonth { get; set; }
        public virtual uint EndMonth { get; set; }
        public virtual uint Tenure { get; set; }
    }
}
