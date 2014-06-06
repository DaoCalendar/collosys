using System;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Generic
{
    public class GTaxDetail : Entity 
    {
        public virtual GTaxesList GTaxesList { get; set; }

        public virtual string ApplicableTo { get; set; }
        public virtual string IndustryZone { get; set; }
        public virtual string Country { get; set; }
        public virtual string State { get; set; }
        public virtual string District { get; set; }
        public virtual int Priority { get; set; }
        public virtual UInt64 TaxId { get; set; }
        public virtual decimal Percentage { get; set; }

        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
    }
}