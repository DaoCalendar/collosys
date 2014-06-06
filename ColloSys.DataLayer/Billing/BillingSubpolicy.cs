using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Billing
{
    public class BillingSubpolicy : Entity
    {
        public virtual IList<BillingRelation> BillingRelations { get; set; }
        public virtual IList<BillDetail> BillDetails { get; set; }
        public virtual IList<BillTokens> BillTokens { get; set; } 

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual ScbEnums.Products Products { get; set; }
        public virtual string GroupBy { get; set; }

        public virtual ColloSysEnums.PayoutSubpolicyType PayoutSubpolicyType { get; set; }
        public virtual ColloSysEnums.OutputType OutputType { get; set; }
        public virtual ColloSysEnums.PolicyType PolicyType { get; set; }

        public virtual bool IsActive { get; set; }
        public virtual bool IsInUse { get; set; }
    }
}