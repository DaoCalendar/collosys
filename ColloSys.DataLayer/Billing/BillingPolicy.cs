using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Billing
{
    public class BillingPolicy : Entity
    {
        public virtual IList<BillingRelation> BillingRelations { get; set; }
        public virtual IList<BillTokens> BillTokens { get; set; }
        public virtual IList<BillDetail> BillDetails { get; set; }

        public virtual string Name { get; set; }
        public virtual ScbEnums.Products Products { get; set; }
        public virtual ScbEnums.Category Category { get; set; }
        public virtual ColloSysEnums.PolicyType PolicyType { get; set; }
        public virtual ColloSysEnums.PolicyOn PolicyFor { get; set; }
        public virtual Guid? PolicyForId { get; set; }
    }
}