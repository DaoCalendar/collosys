using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Mapping
{
    public class HoldingPolicy : Entity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual ColloSysEnums.ApplyOn ApplyOn { get; set; }
        public virtual ScbEnums.Products Products { get; set; }
        public virtual ColloSysEnums.RuleForHolding Rule { get; set; }
        public virtual decimal Value { get; set; }
        public virtual string TransactionType { get; set; }
        public virtual int StartMonth { get; set; }
        public virtual int Tenure { get; set; }

        public virtual IList<ActivateHoldingPolicy> ActivateHoldingPolicies { get; set; } 
    }
}
